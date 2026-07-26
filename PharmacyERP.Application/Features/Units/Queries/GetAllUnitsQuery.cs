using MediatR;
using PharmacyERP.Application.Common.Models;

public class GetAllUnitsQuery : IRequest<Result<List<UnitDto>>>
{
}