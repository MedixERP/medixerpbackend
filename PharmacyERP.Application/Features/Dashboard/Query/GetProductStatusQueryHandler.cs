using MediatR;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Domain.Entities;

public class GetProductStatusQueryHandler
    : IRequestHandler<GetProductStatusQuery, List<ProductStatusDto>>
{
    private readonly IUnitOfWork _uow;

    public GetProductStatusQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<List<ProductStatusDto>> Handle(
        GetProductStatusQuery request,
        CancellationToken cancellationToken)
    {
        var products = await _uow.Repository<Product>()
            .GetAllAsync();

        return products.Select(p => new ProductStatusDto
        {
            ProductId = p.Id,
            ProductName = p.Name,
            Stock = p.MinStockLevel,
            Status =
                p.MinStockLevel == 0 ? "OutOfStock" :
                p.MinStockLevel <= p.MinStockLevel ? "LowStock" :
                "Normal"
        }).ToList();
    }
}