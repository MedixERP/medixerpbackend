using MediatR;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Application.Features.Product.Query;

public class GetProductByIdWithBatchesHandler
    : IRequestHandler<GetProductByIdWithBatchesQuery, Result<ProductDto>>
{
    private readonly IUnitOfWork _uow;

    public GetProductByIdWithBatchesHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<ProductDto>> Handle(
        GetProductByIdWithBatchesQuery request,
        CancellationToken cancellationToken)
    {
        var product = await _uow.Products.Query()
            .Include(p => p.ProductBatches)
            .FirstOrDefaultAsync(p => p.Id == request.Id);

        if (product == null)
            return Result<ProductDto>.Failure("Product not found", 404);

        var dto = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Barcode = product.Barcode,
            SalePrice = product.SalePrice,
            Batches = product.ProductBatches.Select(b => new ProductBatchDto
            {
                BatchNumber = b.BatchNumber,
                Quantity = b.Quantity,
                ExpiryDate = b.ExpiryDate
            }).ToList()
        };

        return Result<ProductDto>.Success(dto);
    }
}