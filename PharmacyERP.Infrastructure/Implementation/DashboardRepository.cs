using Microsoft.EntityFrameworkCore;
using PharmacyERP.Infrastructure.Persistence;

public class DashboardRepository : IDashboardRepository
{
    private readonly ApplicationDbContext _context;

    public DashboardRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<decimal> GetTotalSalesAsync()
    {
        return await _context.Invoices
            .SumAsync(x => (decimal?)x.FinalAmount) ?? 0;
    }

    public async Task<int> GetTotalInvoicesAsync()
    {
        return await _context.Invoices.CountAsync();
    }

    public async Task<int> GetLowStockCountAsync()
    {
        return await _context.Products
            .CountAsync(x => x.MinStockLevel >= x.MinStockLevel);
    }

    public async Task<int> GetExpiredProductsCountAsync()
    {
        return await _context.ProductBatches
            .CountAsync(x => x.ExpiryDate < DateTime.UtcNow);
    }

    public async Task<List<TopSellingProductDto>> GetTopSellingProductsAsync(int count)
    {
        return await _context.InvoiceItems
            .Include(x => x.Product)
            .GroupBy(x => new { x.ProductId, x.Product.Name })
            .Select(g => new TopSellingProductDto
            {
                ProductId = g.Key.ProductId,
                ProductName = g.Key.Name,
                QuantitySold = g.Sum(x => x.Quantity),
                Revenue = g.Sum(x => x.Total)
            })
            .OrderByDescending(x => x.QuantitySold)
            .Take(count)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalProfitAsync()
    {
        var sales = await _context.InvoiceItems.SumAsync(x => (decimal?)x.Total) ?? 0;
        var cost = await _context.InvoiceItems.SumAsync(x =>
            x.Quantity * x.Product.PurchasePrice);

        return sales - cost;
    }

    public async Task<List<ProductStatusDto>> GetProductStatusAsync()
    {
        return await _context.Products
            .Select(p => new ProductStatusDto
            {
                ProductId = p.Id,
                ProductName = p.Name,
                Stock = p.MinStockLevel,
                Status =
                    p.MinStockLevel == 0 ? "OutOfStock" :
                    p.MinStockLevel <= p.MinStockLevel ? "Low" :
                    "Normal"
            })
            .ToListAsync();
    }

    public async Task<List<MonthlySalesDto>> GetMonthlySalesAsync()
    {
        return await _context.Invoices
            .GroupBy(x => x.CreatedAt.Month)
            .Select(g => new MonthlySalesDto
            {
                Month = g.Key.ToString(),
                Sales = g.Sum(x => x.FinalAmount)
            })
            .ToListAsync();
    }
    public async Task<ProfitReportDto> GetProfitReportAsync()
    {
        var invoices = await _context.InvoiceItems
            .Include(x => x.Product)
            .ToListAsync();

        var sales = invoices.Sum(x =>
            x.UnitPrice * x.Quantity);

        var cost = invoices.Sum(x =>
            x.Product.PurchasePrice * x.Quantity);

        return new ProfitReportDto
        {
            TotalSales = sales,
            TotalCost = cost,
            Profit = sales - cost
        };
    }
    public async Task<TodaySalesDto> GetTodaySalesAsync()
    {
        var today = DateTime.Today;

        var invoices = await _context.Invoices
            .Include(x => x.InvoiceItems)
            .Where(x =>
                x.CreatedAt.Date == today)
            .ToListAsync();

        return new TodaySalesDto
        {
            InvoiceCount = invoices.Count,

            TotalSales = invoices
                .Sum(x => x.TotalAmount)
        };
    }
    public async Task<AlertsDto> GetAlertsAsync()
    {
        var today = DateTime.Today;

        var lowStock = await _context.Products
            .Include(x => x.ProductBatches)
            .Where(x =>
                x.ProductBatches.Sum(b =>
                    b.Quantity)
                <= x.MinStockLevel)
            .CountAsync();

        var expired = await _context.ProductBatches
            .CountAsync(x =>
                x.ExpiryDate < today);

        var nearExpiry = await _context.ProductBatches
            .CountAsync(x =>
                x.ExpiryDate >= today &&
                x.ExpiryDate <= today.AddDays(30));

        return new AlertsDto
        {
            LowStockCount = lowStock,

            ExpiredCount = expired,

            NearExpiryCount = nearExpiry
        };
    }
}