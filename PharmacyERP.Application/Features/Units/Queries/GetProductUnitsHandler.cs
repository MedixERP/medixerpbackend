using MediatR;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;

public class GetProductUnitsHandler
    : IRequestHandler<GetProductUnitsQuery, Result<List<ProductUnitDto>>>
{
    private readonly IUnitOfWork _uow;

    public GetProductUnitsHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<List<ProductUnitDto>>> Handle(
        GetProductUnitsQuery request,
        CancellationToken cancellationToken)
    {
        var data = await _uow.Repository<ProductUnit>()
            .Query()
            .AsNoTracking()
            .Include(x => x.Product)
            .Include(x => x.Unit)
            .Where(x => x.ProductId == request.ProductId && !x.IsDeleted)
            .Select(x => new ProductUnitDto
            {
                Id = x.Id,
                ProductId = x.ProductId,
                ProductName = x.Product.Name,
                UnitId = x.UnitId,
                UnitName = x.Unit.Name,
                UnitSymbol = x.Unit.Symbol,
                ConversionFactor = x.ConversionFactor,
                IsBaseUnit = x.IsBaseUnit
            })
            .ToListAsync(cancellationToken);

        return Result<List<ProductUnitDto>>.Success(
            data, "Product units retrieved successfully");
    }
}