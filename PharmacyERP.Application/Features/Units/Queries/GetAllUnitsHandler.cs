using MediatR;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;

public class GetAllUnitsHandler
    : IRequestHandler<GetAllUnitsQuery, Result<List<UnitDto>>>
{
    private readonly IUnitOfWork _uow;

    public GetAllUnitsHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<List<UnitDto>>> Handle(
        GetAllUnitsQuery request,
        CancellationToken cancellationToken)
    {
        var units = await _uow.Repository<PharmacyERP.Domain.Entities.Unit>()
            .Query()
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .Select(x => new UnitDto
            {
                Id = x.Id,
                Name = x.Name,
                Symbol = x.Symbol
            })
            .ToListAsync(cancellationToken);

        return Result<List<UnitDto>>.Success(units, "Units retrieved successfully");
    }
}