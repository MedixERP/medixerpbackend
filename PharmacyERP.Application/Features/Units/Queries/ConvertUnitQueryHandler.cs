using MediatR;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;

public class ConvertUnitQueryHandler
    : IRequestHandler<ConvertUnitQuery, Result<ConvertUnitResultDto>>
{
    private readonly IUnitOfWork _uow;

    public ConvertUnitQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<ConvertUnitResultDto>> Handle(
        ConvertUnitQuery request,
        CancellationToken cancellationToken)
    {
        var fromUnit = await _uow.Repository<ProductUnit>()
            .Query()
            .Include(x => x.Unit)
            .FirstOrDefaultAsync(
                x => x.ProductId == request.ProductId
                  && x.UnitId == request.FromUnitId,
                cancellationToken);

        var toUnit = await _uow.Repository<ProductUnit>()
            .Query()
            .Include(x => x.Unit)
            .FirstOrDefaultAsync(
                x => x.ProductId == request.ProductId
                  && x.UnitId == request.ToUnitId,
                cancellationToken);

        if (fromUnit == null)
            return Result<ConvertUnitResultDto>.Failure(
                "From unit not found for this product", 404);

        if (toUnit == null)
            return Result<ConvertUnitResultDto>.Failure(
                "To unit not found for this product", 404);

        var baseQuantity = request.Quantity * fromUnit.ConversionFactor;
        var convertedQuantity = baseQuantity / toUnit.ConversionFactor;

        return Result<ConvertUnitResultDto>.Success(
            new ConvertUnitResultDto
            {
                FromUnit = fromUnit.Unit.Name,
                ToUnit = toUnit.Unit.Name,
                OriginalQuantity = request.Quantity,
                ConvertedQuantity = convertedQuantity
            },
            "Converted successfully");
    }
}