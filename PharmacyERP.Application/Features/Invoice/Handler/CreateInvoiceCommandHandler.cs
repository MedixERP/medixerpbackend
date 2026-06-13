using MediatR;
using Microsoft.AspNetCore.Http;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;
using PharmacyERP.Domain.Enums;
using System.Security.Claims;

public class CreateInvoiceCommandHandler
    : IRequestHandler<CreateInvoiceCommand, Result<int>>
{
    private readonly IUnitOfWork _uow;
    private readonly IHttpContextAccessor _http;

    public CreateInvoiceCommandHandler(
        IUnitOfWork uow,
        IHttpContextAccessor http)
    {
        _uow = uow;
        _http = http;
    }

    public async Task<Result<int>> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        var user = _http.HttpContext?.User;

        if (user == null || !user.Identity!.IsAuthenticated)
            return Result<int>.Failure("Unauthorized", 401);

        if (!user.IsInRole("Admin") && !user.IsInRole("Cashier"))
            return Result<int>.Failure("Forbidden", 403);

        if (request.Items == null || !request.Items.Any())
            return Result<int>.Failure("Empty invoice", 400);

        using var transaction = await _uow.BeginTransactionAsync();

        try
        {
            var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var invoice = new Invoice
            {
                CustomerId = request.CustomerId,
                InvoiceNumber = await _uow.Invoices.GenerateInvoiceNumberAsync(),
                CreatedByUserId = userId,
                CreatedAt = DateTime.UtcNow,
                InvoiceItems = new List<InvoiceItem>()
            };

            decimal total = 0;

            foreach (var item in request.Items)
            {
                var batch = await _uow.ProductBatches
                    .GetOldestBatchAsync(item.ProductId);

                if (batch == null || batch.Quantity < item.Quantity)
                    return Result<int>.Failure("Insufficient stock", 400);

                var product = await _uow.Products.GetByIdAsync(item.ProductId);
                if (product == null)
                    return Result<int>.Failure("Product not found", 404);

                var price = product.SalePrice * item.Quantity;
                total += price;

                batch.Quantity -= item.Quantity;

                invoice.InvoiceItems.Add(new InvoiceItem
                {
                    ProductId = product.Id,
                    BatchId = batch.Id,
                    Quantity = item.Quantity,
                    UnitPrice = product.SalePrice,
                    Total = price
                });
            }

            invoice.TotalAmount = total;
            invoice.FinalAmount = total;

            await _uow.Invoices.AddAsync(invoice);
            await _uow.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync();

            return Result<int>.Success(invoice.Id, "Created");
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}