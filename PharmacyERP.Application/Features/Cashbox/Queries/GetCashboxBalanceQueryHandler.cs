using MediatR;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;
using PharmacyERP.Domain.Enums;

public class GetCashboxBalanceQueryHandler
    : IRequestHandler<GetCashboxBalanceQuery, Result<CashboxBalanceDto>>
{
    private readonly IUnitOfWork _uow;

    public GetCashboxBalanceQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<CashboxBalanceDto>> Handle(
        GetCashboxBalanceQuery request,
        CancellationToken cancellationToken)
    {
        var transactions = await _uow
            .Repository<CashboxTransaction>()
            .GetAllAsync();

        var totalIn = transactions
            .Where(x => x.Type == CashboxTransactionType.In)
            .Sum(x => x.Amount);

        var totalOut = transactions
            .Where(x => x.Type == CashboxTransactionType.Out)
            .Sum(x => x.Amount);

        return Result<CashboxBalanceDto>.Success(
            new CashboxBalanceDto
            {
                TotalIn = totalIn,
                TotalOut = totalOut,
                Balance = totalIn - totalOut
            },
            "Balance retrieved successfully");
    }
}