using MediatR;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;

public class ScanBarcodeToInvoiceCommandHandler
    : IRequestHandler<ScanBarcodeToInvoiceCommand, Result<int>>
{
    private readonly IUnitOfWork _uow;

    public ScanBarcodeToInvoiceCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<int>> Handle(
        ScanBarcodeToInvoiceCommand request,
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

        var product = await _uow.Products
            .Query()
            .Include(x => x.ProductBatches)
            .FirstOrDefaultAsync(
                x => x.Barcode == request.Barcode,
                cancellationToken);

        if (product == null)
            return Result<int>.Failure("Invalid barcode", 404);

        var batch = product.ProductBatches
            .Where(x =>
                x.Quantity > 0 &&
                x.ExpiryDate > DateTime.UtcNow &&
                !x.IsDeleted)
            .OrderBy(x => x.ExpiryDate)
            .FirstOrDefault();

        if (batch == null)
            return Result<int>.Failure("No valid stock", 400);

        if (batch.Quantity < request.Quantity)
            return Result<int>.Failure("Insufficient stock", 400);

        var existingItem = invoice.InvoiceItems
            .FirstOrDefault(x =>
                x.ProductId == product.Id &&
                x.BatchId == batch.Id);

        if (existingItem != null)
        {
            existingItem.Quantity += request.Quantity;
            existingItem.Total = existingItem.Quantity * existingItem.UnitPrice;
        }
        else
        {
            invoice.InvoiceItems.Add(new InvoiceItem
            {
                ProductId = product.Id,
                BatchId = batch.Id,
                Quantity = request.Quantity,
                UnitPrice = product.SalePrice,
                Total = request.Quantity * product.SalePrice
            });
        }

        if (batch.Quantity - request.Quantity < 0)
            return Result<int>.Failure("Stock would go negative", 400);

        batch.Quantity -= request.Quantity;

        _uow.ProductBatches.Update(batch);

        invoice.TotalAmount = invoice.InvoiceItems.Sum(x => x.Total);
        invoice.FinalAmount = invoice.TotalAmount;

        await _uow.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(invoice.Id);
    }
}