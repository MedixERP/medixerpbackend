using MediatR;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;

public class SearchProductsQueryHandler
    : IRequestHandler<SearchProductsQuery, Result<List<ProductSearchDto>>>
{
    private readonly IUnitOfWork _uow;

    public SearchProductsQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<List<ProductSearchDto>>> Handle(
     SearchProductsQuery request,
     CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Keyword))
            return Result<List<ProductSearchDto>>
                .Failure("Keyword is required", 400);

        var products = await _uow.Products.SmartSearchAsync(request.Keyword);

        var result = new List<ProductSearchDto>();

        foreach (var x in products)
        {
            var stock = await _uow.Products.GetTotalStockAsync(x.Id);

            result.Add(new ProductSearchDto
            {
                Id = x.Id,
                Name = x.Name,
                ScientificName = x.ScientificName,
                Barcode = x.Barcode,
                SalePrice = x.SalePrice,
                Stock = stock,
                IsLowStock = stock <= x.MinStockLevel
            });
        }

        return Result<List<ProductSearchDto>>.Success(result);
    }
}