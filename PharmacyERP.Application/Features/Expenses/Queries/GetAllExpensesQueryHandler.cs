using MediatR;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;

public class GetAllExpensesQueryHandler
    : IRequestHandler<GetAllExpensesQuery, Result<PaginatedResult<ExpenseDto>>>
{
    private readonly IUnitOfWork _uow;

    public GetAllExpensesQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<PaginatedResult<ExpenseDto>>> Handle(
        GetAllExpensesQuery request,
        CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;

        var query = _uow.Repository<Expense>().Query();

        if (request.FromDate.HasValue)
            query = query.Where(x => x.PaidAt >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            query = query.Where(x => x.PaidAt <= request.ToDate.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var data = await query
            .AsNoTracking()
            .Include(x => x.CreatedByUser)
            .OrderByDescending(x => x.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new ExpenseDto
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                Amount = x.Amount,
                PaidAt = x.PaidAt,
                CreatedBy = x.CreatedByUser != null
                    ? x.CreatedByUser.FullName : "System Admin"
            })
            .ToListAsync(cancellationToken);

        return Result<PaginatedResult<ExpenseDto>>.Success(
            new PaginatedResult<ExpenseDto>(
                data, totalCount, pageNumber, pageSize),
            "Expenses retrieved successfully");
    }
}