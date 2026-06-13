using MediatR;
using PharmacyERP.Application.Common.Interfaces;

public class GetDashboardQueryHandler
    : IRequestHandler<GetDashboardQuery, DashboardSummaryDto>
{
    private readonly IUnitOfWork _uow;

    public GetDashboardQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<DashboardSummaryDto> Handle(
        GetDashboardQuery request,
        CancellationToken cancellationToken)
    {
        return new DashboardSummaryDto
        {
            TotalSales = await _uow.Dashboard.GetTotalSalesAsync(),
            TotalInvoices = await _uow.Dashboard.GetTotalInvoicesAsync(),
            LowStockCount = await _uow.Dashboard.GetLowStockCountAsync(),
            ExpiredProductsCount = await _uow.Dashboard.GetExpiredProductsCountAsync(),
            TotalProfit = await _uow.Dashboard.GetTotalProfitAsync()
        };
    }
}