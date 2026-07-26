using MediatR;
using PharmacyERP.Application.Common.Models;

public class GetCashboxTransactionsQuery
    : PaginationRequest,
      IRequest<Result<PaginatedResult<CashboxTransactionDto>>>
{
    public string? Type { get; set; }
    public string? Source { get; set; }
}