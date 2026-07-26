using MediatR;
using PharmacyERP.Application.Common.Models;

public class GetAllExpensesQuery
    : PaginationRequest,
      IRequest<Result<PaginatedResult<ExpenseDto>>>
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}