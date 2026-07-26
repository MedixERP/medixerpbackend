using MediatR;
using PharmacyERP.Application.Common.Models;

public class GetCashboxBalanceQuery : IRequest<Result<CashboxBalanceDto>>
{
}