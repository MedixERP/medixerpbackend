using MediatR;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;
using PharmacyERP.Domain.Enums;

public class RemoveInvoiceItemCommandHandler
    : IRequestHandler<RemoveInvoiceItemCommand, Result<int>>
{
    private readonly IUnitOfWork _uow;

    public RemoveInvoiceItemCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<int>> Handle(
    RemoveInvoiceItemCommand request,
    CancellationToken cancellationToken)
    {
        var invoice = await _uow.Repository<Invoice>()
            .Query()
            .Include(x => x.InvoiceItems)
            .FirstOrDefaultAsync(
                x => x.Id == request.InvoiceId,
                cancellationToken);

        if (invoice == null)
            return Result<int>.Failure("Invoice not found", 404);

        if (invoice.IsCancelled)
            return Result<int>.Failure("Cannot modify cancelled invoice", 400);

        var item = invoice.InvoiceItems
            .FirstOrDefault(x => x.Id == request.InvoiceItemId);

        if (item == null)
            return Result<int>.Failure("Item not found", 404);

        var batch = await _uow.ProductBatches
            .GetByIdAsync(item.BatchId);

        if (batch == null)
            return Result<int>.Failure("Batch not found", 404);

        var beforeQty = batch.Quantity;

        batch.Quantity += item.Quantity;

        _uow.ProductBatches.Update(batch);

        await _uow.Repository<InventoryMovement>().AddAsync(new InventoryMovement
        {
            ProductId = item.ProductId,
            BatchId = item.BatchId,
            Quantity = item.Quantity,
            Type = InventoryMovementType.SalesReturn,
            Reason = "Invoice item removed",
            BeforeQuantity = beforeQty,
            AfterQuantity = batch.Quantity,
            ReferenceType = "InvoiceRemove",
            ReferenceId = invoice.Id,
            CreatedAt = DateTime.UtcNow
        });

        invoice.InvoiceItems.Remove(item);

        invoice.TotalAmount = invoice.InvoiceItems.Sum(x => x.Total);
        invoice.FinalAmount = invoice.TotalAmount;

        await _uow.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(invoice.Id);
    }
}