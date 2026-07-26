using MediatR;
using PharmacyERP.Application.Common.Models;

public class RejectDrugOrderCommand : IRequest<Result<string>>
{
    public int OrderId { get; set; }
    public string? Reason { get; set; }
}