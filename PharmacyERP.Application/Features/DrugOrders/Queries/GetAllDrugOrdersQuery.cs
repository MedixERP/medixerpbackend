using MediatR;
using PharmacyERP.Application.Common.Models;

public class GetAllDrugOrdersQuery
    : PaginationRequest,
      IRequest<Result<PaginatedResult<DrugOrderDto>>>
{
    public string? Status { get; set; }
    public int? CompanyId { get; set; }
}