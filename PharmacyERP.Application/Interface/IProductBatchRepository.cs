using PharmacyERP.Domain.Entities;

namespace PharmacyERP.Application.Common.Interfaces.Repositories;

public interface IProductBatchRepository
    : IGenericRepository<ProductBatch>
{
    Task<List<ProductBatch>> GetByProductIdAsync(int productId);

    Task<ProductBatch?> GetAvailableBatchAsync(
        int productId,
        int batchId);

    Task<List<ProductBatch>> GetExpiredBatchesAsync();

    Task<List<ProductBatch>> GetNearExpiryBatchesAsync(int days);

    Task<int> GetTotalStockAsync(int productId);
    Task<ProductBatch?> GetOldestBatchAsync(int productId);
}