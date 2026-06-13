public interface IDashboardRepository
{
    Task<decimal> GetTotalSalesAsync();
    Task<int> GetTotalInvoicesAsync();
    Task<int> GetLowStockCountAsync();
    Task<int> GetExpiredProductsCountAsync();

    Task<List<TopSellingProductDto>> GetTopSellingProductsAsync(int count);

    Task<decimal> GetTotalProfitAsync();

    Task<List<ProductStatusDto>> GetProductStatusAsync();

    Task<List<MonthlySalesDto>> GetMonthlySalesAsync();
    Task<ProfitReportDto> GetProfitReportAsync();
    Task<TodaySalesDto> GetTodaySalesAsync();
    Task<AlertsDto> GetAlertsAsync();
}