using PharmacyERP.Domain.Entities;
using System.Linq.Expressions;

namespace PharmacyERP.Application.Common.Interfaces.Repositories;

public interface IProductRepository : IGenericRepository<Product>
{


    Task<List<Product>> SmartSearchAsync(string keyword);
    Task<List<Product>> GetByCategoryAsync(int categoryId);

    Task<Product?> GetByBarcodeAsync(string barcode);

 

    Task<int> GetTotalStockAsync(int productId);

    Task<bool> IsLowStockAsync(int productId);

    Task<List<Product>> GetLowStockProductsAsync();

   

    Task<List<ProductBatch>> GetBatchesAsync(int productId);

    Task<ProductBatch?> GetOldestBatchAsync(int productId);

    Task DeductFromBatchAsync(int productId, int quantity);

 

    Task<bool> IsBarcodeExistsAsync(string barcode);
    Task<Product?> GetByIdWithBatchesAsync(int id);

    Task<List<Product>> GetAllWithBatchesAsync();
    Task<bool> AnyAsync(Expression<Func<Product, bool>> predicate);


}