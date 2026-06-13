using MediatR;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Models;

public class ScanBarcodeQueryHandler
    : IRequestHandler<ScanBarcodeQuery, Result<ScannedProductDto>>
{
    private readonly IUnitOfWork _uow;

    public ScanBarcodeQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<ScannedProductDto>> Handle(
      ScanBarcodeQuery request,
      CancellationToken cancellationToken)
    {
        var product = await _uow.Products
            .Query()
            .Include(x => x.ProductBatches)
            .FirstOrDefaultAsync(
                x => x.Barcode == request.Barcode,
                cancellationToken);

        if (product == null)
            return Result<ScannedProductDto>.Failure("Invalid barcode", 404);

        var batch = product.ProductBatches
            .Where(x =>
                x.Quantity > 0 &&
                x.ExpiryDate > DateTime.UtcNow &&
                !x.IsDeleted)
            .OrderBy(x => x.ExpiryDate)
            .ThenBy(x => x.Id)
            .FirstOrDefault();

        if (batch == null)
            return Result<ScannedProductDto>.Failure("No valid stock available", 400);

        return Result<ScannedProductDto>.Success(new ScannedProductDto
        {
            ProductId = product.Id,
            ProductName = product.Name,
            Barcode = product.Barcode,
            Price = product.SalePrice,

            BatchId = batch.Id,
            BatchNumber = batch.BatchNumber,
            ExpiryDate = batch.ExpiryDate,
            AvailableQuantity = batch.Quantity
        });
    }
}