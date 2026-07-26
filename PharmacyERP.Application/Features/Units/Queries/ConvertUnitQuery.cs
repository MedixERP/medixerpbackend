using MediatR;
using PharmacyERP.Application.Common.Models;

public class ConvertUnitQuery : IRequest<Result<ConvertUnitResultDto>>
{
    public int ProductId { get; set; }
    public int FromUnitId { get; set; }
    public int ToUnitId { get; set; }
    public decimal Quantity { get; set; }
}