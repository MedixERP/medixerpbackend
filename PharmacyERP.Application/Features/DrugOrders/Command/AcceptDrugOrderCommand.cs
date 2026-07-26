using MediatR;
using PharmacyERP.Application.Common.Models;

public class AcceptDrugOrderCommand : IRequest<Result<string>>
{
    public int OrderId { get; set; }
}