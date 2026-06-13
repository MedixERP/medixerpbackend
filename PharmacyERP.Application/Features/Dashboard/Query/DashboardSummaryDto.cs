public class DashboardSummaryDto
{
    public decimal TotalSales { get; set; }
    public int TotalInvoices { get; set; }
    public int LowStockCount { get; set; }
    public int ExpiredProductsCount { get; set; }
    public decimal TotalProfit { get; set; }
}