using MediatR;
using PharmacyERP.Application.Common.Models;

public class GetAllSalesReturnsQuery
    : PaginationRequest,
      IRequest<Result<PaginatedResult<SalesReturnDto>>>
{
    public string? Keyword { get; set; }
}