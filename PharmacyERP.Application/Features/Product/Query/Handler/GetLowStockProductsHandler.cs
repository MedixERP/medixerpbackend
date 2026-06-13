using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;

public class GetLowStockProductsHandler
    : IRequestHandler<GetLowStockProductsQuery, Result<List<ProductDto>>>
{
    private readonly IUnitOfWork _uow;
    private readonly IHttpContextAccessor _http;

    public GetLowStockProductsHandler(IUnitOfWork uow, IHttpContextAccessor http)
    {
        _uow = uow;
        _http = http;
    }

    public async Task<Result<List<ProductDto>>> Handle(
        GetLowStockProductsQuery request,
        CancellationToken cancellationToken)
    {
        var products = await _uow.Products
            .GetAllWithBatchesAsync();

        var result = products
            .Select(p =>
            {
                var stock = p.ProductBatches.Sum(b => b.Quantity);

                return new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Barcode = p.Barcode,
                    SalePrice = p.SalePrice,
                    TotalStock = stock,
                    IsLowStock = stock <= p.MinStockLevel
                };
            })
            .Where(p => p.IsLowStock)
            .ToList();

        return Result<List<ProductDto>>.Success(result);
    }
}