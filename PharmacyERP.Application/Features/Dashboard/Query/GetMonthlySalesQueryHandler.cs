using MediatR;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Domain.Entities;

public class GetMonthlySalesQueryHandler
    : IRequestHandler<GetMonthlySalesQuery, List<MonthlySalesDto>>
{
    private readonly IUnitOfWork _uow;

    public GetMonthlySalesQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<List<MonthlySalesDto>> Handle(
        GetMonthlySalesQuery request,
        CancellationToken cancellationToken)
    {
        var invoices = await _uow.Repository<Invoice>()
            .GetAllAsync();

        var result = invoices
            .GroupBy(x => x.CreatedAt.Month)
            .Select(g => new MonthlySalesDto
            {
                Month = g.Key.ToString(),
               Sales = g.Sum(x => x.FinalAmount)
            })
            .OrderBy(x => int.Parse(x.Month))
            .ToList();

        return result;
    }
}