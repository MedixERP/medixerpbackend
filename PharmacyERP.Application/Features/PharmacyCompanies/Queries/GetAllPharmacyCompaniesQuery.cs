using MediatR;
using PharmacyERP.Application.Common.Models;

public class GetAllPharmacyCompaniesQuery
    : PaginationRequest,
      IRequest<Result<PaginatedResult<PharmacyCompanyDto>>>
{
    public string? Keyword { get; set; }
    public bool? IsActive { get; set; }
}