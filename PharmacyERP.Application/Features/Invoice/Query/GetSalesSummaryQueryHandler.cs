using MediatR;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Domain.Entities;

public class GetSalesSummaryQueryHandler : IRequestHandler<GetSalesSummaryQuery, SalesSummaryDto>
{
    private readonly IUnitOfWork _uow;

    public GetSalesSummaryQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<SalesSummaryDto> Handle(GetSalesSummaryQuery request, CancellationToken cancellationToken)
    {
        var invoices = await _uow.Repository<Invoice>()
            .Query()
            .Where(x => x.CreatedAt.Date == request.Date.Date && !x.IsCancelled)
            .ToListAsync(cancellationToken);

        return new SalesSummaryDto
        {
            Date = request.Date,
            TotalSales = invoices.Sum(x => x.FinalAmount),
            TotalInvoices = invoices.Count
        };
    }
}