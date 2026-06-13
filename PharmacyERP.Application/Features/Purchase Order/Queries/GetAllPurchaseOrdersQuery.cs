using MediatR;
using PharmacyERP.Application.Common.Models;

public class GetAllPurchaseOrdersQuery
    : PaginationRequest,
      IRequest<Result<PaginatedResult<PurchaseOrderDto>>>
{
    public int? SupplierId { get; set; }
    public string? Status { get; set; }
}