using MediatR;
using PharmacyERP.Application.Common.Models;

public class GetAllInvoicesQuery
    : PaginationRequest,
      IRequest<Result<PaginatedResult<InvoiceDto>>>
{
    public int? CustomerId { get; set; }
    public bool? IsCancelled { get; set; }
}