using MediatR;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Domain.Entities;

public class GetTodaySalesHandler
    : IRequestHandler<GetTodaySalesQuery, TodaySalesDto>
{
    private readonly IUnitOfWork _uow;

    public GetTodaySalesHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<TodaySalesDto> Handle(
        GetTodaySalesQuery request,
        CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        var invoices = await _uow.Repository<Invoice>()
            .Query()
            .Where(x =>
                !x.IsCancelled &&
                x.CreatedAt >= today &&
                x.CreatedAt < tomorrow)
            .ToListAsync(cancellationToken);

        return new TodaySalesDto
        {
            TotalSales = invoices.Sum(x => x.FinalAmount),
            InvoiceCount = invoices.Count
        };
    }
}