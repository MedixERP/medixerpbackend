using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces.Repositories;
using PharmacyERP.Domain.Entities;
using PharmacyERP.Infrastructure.Persistence;

namespace PharmacyERP.Infrastructure.Repositories;

public class ProductBatchRepository
    : GenericRepository<ProductBatch>,
      IProductBatchRepository
{
    private readonly ApplicationDbContext _context;

    public ProductBatchRepository(ApplicationDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<List<ProductBatch>> GetByProductIdAsync(int productId)
    {
        return await _context.ProductBatches
            .Include(x => x.Product)
            .Where(x =>
                x.ProductId == productId &&
                !x.IsDeleted)
            .ToListAsync();
    }

    public async Task<ProductBatch?> GetAvailableBatchAsync(
        int productId,
        int batchId)
    {
        return await _context.ProductBatches
            .FirstOrDefaultAsync(x =>
                x.ProductId == productId &&
                x.Id == batchId &&
                x.Quantity > 0 &&
                x.ExpiryDate > DateTime.UtcNow &&
                !x.IsDeleted);
    }

    public async Task<List<ProductBatch>> GetExpiredBatchesAsync()
    {
        return await _context.ProductBatches
            .Include(x => x.Product)
           .Where(x =>
           x.ExpiryDate < DateTime.UtcNow &&
           x.Quantity > 0 &&
           !x.IsDeleted)
            .ToListAsync();
    }

    public async Task<List<ProductBatch>> GetNearExpiryBatchesAsync(int days)
    {
        var targetDate = DateTime.UtcNow.AddDays(days);

        return await _context.ProductBatches
            .Include(x => x.Product)
           .Where(x =>
    x.ExpiryDate <= targetDate &&
    x.ExpiryDate > DateTime.UtcNow &&
    x.Quantity > 0 &&
    !x.IsDeleted)
            .ToListAsync();
    }

    public async Task<int> GetTotalStockAsync(int productId)
    {
        return await _context.ProductBatches
            .Where(x =>
                x.ProductId == productId &&
                !x.IsDeleted)
            .SumAsync(x => x.Quantity);
    }
    public async Task<ProductBatch?> GetOldestBatchAsync(int productId)
    {
        return await _context.ProductBatches
            .Where(x =>
                x.ProductId == productId &&
                x.Quantity > 0 &&
                !x.IsDeleted)
            .OrderBy(x => x.ExpiryDate) 
            .FirstOrDefaultAsync();
    }
}