using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;
using PharmacyERP.Domain.Enums;
using System.Security.Claims;

public class CreateSalesReturnCommandHandler
    : IRequestHandler<CreateSalesReturnCommand, Result<int>>
{
    private readonly IUnitOfWork _uow;
    private readonly IHttpContextAccessor _http;

    public CreateSalesReturnCommandHandler(IUnitOfWork uow, IHttpContextAccessor http)
    {
        _uow = uow;
        _http = http;
    }

    public async Task<Result<int>> Handle(
     CreateSalesReturnCommand request,
     CancellationToken cancellationToken)
    {
        var user = _http.HttpContext?.User;

        if (user == null || !user.Identity!.IsAuthenticated)
            return Result<int>.Failure("Unauthorized", 401);

        var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var invoice = await _uow.Repository<Invoice>()
            .Query()
            .Include(x => x.InvoiceItems)
            .FirstOrDefaultAsync(x =>
                x.Id == request.InvoiceId &&
                !x.IsDeleted,
                cancellationToken);

        if (invoice == null)
            return Result<int>.Failure("Invoice not found", 404);

        if (invoice.IsCancelled)
            return Result<int>.Failure("Cannot return cancelled invoice", 400);

        var salesReturn = new SalesReturn
        {
            InvoiceId = request.InvoiceId,
            Reason = request.Reason,
            ReturnedByUserId = userId,
            CreatedAt = DateTime.UtcNow,
            SalesReturnItems = new List<SalesReturnItem>()
        };

        decimal total = 0;

        foreach (var item in request.Items)
        {
            if (item.Quantity <= 0)
                return Result<int>.Failure("Invalid return quantity", 400);

            var invoiceItem = invoice.InvoiceItems.FirstOrDefault(x =>
                x.ProductId == item.ProductId &&
                x.BatchId == item.BatchId);

            if (invoiceItem == null)
                return Result<int>.Failure("Invoice item not found", 400);

            // 🔥 prevent over return
            var alreadyReturnedQty = await _uow.Repository<SalesReturnItem>()
                .Query()
                .Where(x =>
                    x.ProductId == item.ProductId &&
                    x.BatchId == item.BatchId)
                .SumAsync(x => (int?)x.Quantity, cancellationToken) ?? 0;

            if (item.Quantity + alreadyReturnedQty > invoiceItem.Quantity)
                return Result<int>.Failure(
                    "Return quantity exceeds sold quantity",
                    400);

            var batch = await _uow.ProductBatches
                .GetByIdAsync(item.BatchId);

            if (batch == null || batch.IsDeleted)
                return Result<int>.Failure("Batch not found", 404);

            var beforeQty = batch.Quantity;

            // 🔥 return stock back
            batch.Quantity += item.Quantity;

            _uow.ProductBatches.Update(batch);

            await _uow.Repository<InventoryMovement>().AddAsync(new InventoryMovement
            {
                ProductId = item.ProductId,
                BatchId = batch.Id,
                Type = InventoryMovementType.SalesReturn,
                Quantity = item.Quantity,
                BeforeQuantity = beforeQty,
                AfterQuantity = batch.Quantity,
                Reason = request.Reason,
                ReferenceType = "SalesReturn",
                ReferenceId = invoice.Id,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            });

            var itemTotal = item.Quantity * invoiceItem.UnitPrice;

            salesReturn.SalesReturnItems.Add(new SalesReturnItem
            {
                ProductId = item.ProductId,
                BatchId = item.BatchId,
                Quantity = item.Quantity,
                UnitPrice = invoiceItem.UnitPrice,
                Total = itemTotal
            });

            total += itemTotal;
        }

        salesReturn.TotalAmount = total;

        await _uow.Repository<SalesReturn>().AddAsync(salesReturn);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(salesReturn.Id);
    }
}