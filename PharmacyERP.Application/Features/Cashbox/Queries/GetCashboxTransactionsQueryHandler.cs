using MediatR;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;
using PharmacyERP.Domain.Enums;

public class GetCashboxTransactionsQueryHandler
    : IRequestHandler<GetCashboxTransactionsQuery,
        Result<PaginatedResult<CashboxTransactionDto>>>
{
    private readonly IUnitOfWork _uow;

    public GetCashboxTransactionsQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }
    public async Task<Result<PaginatedResult<CashboxTransactionDto>>> Handle(
        GetCashboxTransactionsQuery request,
        CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;

        IQueryable<CashboxTransaction> query = _uow.Repository<CashboxTransaction>()
            .Query()
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Type) &&
            Enum.TryParse<CashboxTransactionType>(request.Type, true, out var t))
        {
            query = query.Where(x => x.Type == t);
        }

        if (!string.IsNullOrWhiteSpace(request.Source) &&
            Enum.TryParse<CashboxSource>(request.Source, true, out var s))
        {
            query = query.Where(x => x.Source == s);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var data = await query
            .Include(x => x.CreatedByUser)
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new CashboxTransactionDto
            {
                Id = x.Id,
                Type = x.Type.ToString(),
                Source = x.Source.ToString(),
                Amount = x.Amount,
                Description = x.Description,
                CreatedBy = x.CreatedByUser != null ? x.CreatedByUser.FullName : "",
                CreatedAt = x.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return Result<PaginatedResult<CashboxTransactionDto>>.Success(
            new PaginatedResult<CashboxTransactionDto>(data, totalCount, pageNumber, pageSize),
            "Transactions retrieved successfully");
    }
}