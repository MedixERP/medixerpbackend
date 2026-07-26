using MediatR;
using PharmacyERP.Application.Common.Models;

public class CreateDrugOrderCommand : IRequest<Result<int>>
{
    public int PharmacyCompanyId { get; set; }
    public List<CreateDrugOrderItemDto> Items { get; set; }
}