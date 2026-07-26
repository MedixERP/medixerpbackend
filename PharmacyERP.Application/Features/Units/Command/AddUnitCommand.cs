using MediatR;
using PharmacyERP.Application.Common.Models;

public class AddUnitCommand : IRequest<Result<int>>
{
    public string Name { get; set; }
    public string Symbol { get; set; }
}