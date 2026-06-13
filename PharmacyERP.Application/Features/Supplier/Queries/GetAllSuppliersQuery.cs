using MediatR;
using PharmacyERP.Application.Common.Models;

public class GetAllSuppliersQuery
    : PaginationRequest,
      IRequest<Result<PaginatedResult<SupplierDto>>>
{
    public string? Keyword { get; set; }
}