using MediatR;
using PharmacyERP.Application.Common.Models;

public class UpdateDrugOrderStatusCommand : IRequest<Result<string>>
{
    public int OrderId { get; set; }
    public string Status { get; set; }
}