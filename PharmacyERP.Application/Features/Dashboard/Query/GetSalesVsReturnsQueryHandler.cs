using MediatR;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Domain.Entities;

public class GetSalesVsReturnsQueryHandler
    : IRequestHandler<GetSalesVsReturnsQuery, SalesVsReturnsDto>
{
    private readonly IUnitOfWork _uow;

    public GetSalesVsReturnsQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<SalesVsReturnsDto> Handle(
        GetSalesVsReturnsQuery request,
        CancellationToken cancellationToken)
    {
        var invoices = await _uow.Repository<Invoice>()
            .Query()
            .ToListAsync(cancellationToken);

        var returns = await _uow.Repository<SalesReturn>()
            .Query()
            .ToListAsync(cancellationToken);

        return new SalesVsReturnsDto
        {
            TotalSales = invoices.Sum(x => x.FinalAmount),
            TotalReturns = returns.Sum(x => x.TotalAmount)
        };
    }
}