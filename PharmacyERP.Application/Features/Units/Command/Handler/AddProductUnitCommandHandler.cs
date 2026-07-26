using MediatR;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;

public class AddProductUnitCommandHandler
    : IRequestHandler<AddProductUnitCommand, Result<int>>
{
    private readonly IUnitOfWork _uow;

    public AddProductUnitCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<int>> Handle(
        AddProductUnitCommand request,
        CancellationToken cancellationToken)
    {
        var product = await _uow.Products.GetByIdAsync(request.ProductId);
        if (product == null || product.IsDeleted)
            return Result<int>.Failure("Product not found", 404);

        var unit = await _uow
            .Repository<PharmacyERP.Domain.Entities.Unit>()
            .GetByIdAsync(request.UnitId);
        if (unit == null || unit.IsDeleted)
            return Result<int>.Failure("Unit not found", 404);

        var exists = await _uow.Repository<ProductUnit>()
            .Query()
            .AnyAsync(
                x => x.ProductId == request.ProductId
                  && x.UnitId == request.UnitId,
                cancellationToken);

        if (exists)
            return Result<int>.Failure(
                "This unit is already linked to this product", 400);

        if (request.IsBaseUnit)
        {
            var hasBase = await _uow.Repository<ProductUnit>()
                .Query()
                .AnyAsync(
                    x => x.ProductId == request.ProductId
                      && x.IsBaseUnit,
                    cancellationToken);

            if (hasBase)
                return Result<int>.Failure(
                    "Product already has a base unit", 400);
        }

        var productUnit = new ProductUnit
        {
            ProductId = request.ProductId,
            UnitId = request.UnitId,
            ConversionFactor = request.ConversionFactor,
            IsBaseUnit = request.IsBaseUnit,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.Repository<ProductUnit>().AddAsync(productUnit);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(
            productUnit.Id, "Product unit added successfully");
    }
}