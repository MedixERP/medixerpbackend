using MediatR;
using PharmacyERP.Application.Common.Models;

public class GetProductUnitsQuery : IRequest<Result<List<ProductUnitDto>>>
{
    public int ProductId { get; set; }
}