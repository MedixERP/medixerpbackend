using MediatR;
using PharmacyERP.Application.Common.Models;

public class GetAllCustomersQuery
    : PaginationRequest,
      IRequest<Result<PaginatedResult<CustomerDto>>>
{
    public string? Keyword { get; set; }
    public bool? IsVip { get; set; }
}