using MediatR;
using PharmacyERP.Application.Common.Models;

public class AddProductUnitCommand : IRequest<Result<int>>
{
    public int ProductId { get; set; }
    public int UnitId { get; set; }
    public int ConversionFactor { get; set; }
    public bool IsBaseUnit { get; set; }
}