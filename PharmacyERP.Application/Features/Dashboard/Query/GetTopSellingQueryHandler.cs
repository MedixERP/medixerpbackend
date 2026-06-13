using MediatR;
using PharmacyERP.Application.Common.Interfaces;

public class GetTopSellingQueryHandler
    : IRequestHandler<GetTopSellingQuery, List<TopSellingProductDto>>
{
    private readonly IUnitOfWork _uow;

    public GetTopSellingQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<List<TopSellingProductDto>> Handle(
        GetTopSellingQuery request,
        CancellationToken cancellationToken)
    {
        return await _uow.Dashboard
            .GetTopSellingProductsAsync(request.Count);
    }
}