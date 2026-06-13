using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces.Repositories;
using PharmacyERP.Domain.Entities;
using PharmacyERP.Infrastructure.Persistence;
using System.Linq.Expressions;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

   
    public async Task<List<Product>> SmartSearchAsync(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return new List<Product>();

        keyword = keyword.Trim().ToLower();

        var products = await _context.Products
            .Where(x =>
                !x.IsDeleted &&
                (
                    x.Name.ToLower().Contains(keyword) ||
                    x.Name.ToLower().StartsWith(keyword) ||

                    (x.ScientificName != null &&
                     x.ScientificName.ToLower().Contains(keyword)) ||

                    (x.ScientificName != null &&
                     x.ScientificName.ToLower().StartsWith(keyword))
                )
            )
            .ToListAsync();

        var fuzzyProducts = await _context.Products
            .Where(x => !x.IsDeleted)
            .ToListAsync();

        var fuzzyMatches = fuzzyProducts
            .Where(x =>
                LevenshteinDistance(x.Name.ToLower(), keyword) <= 2 ||
                (
                    !string.IsNullOrWhiteSpace(x.ScientificName) &&
                    LevenshteinDistance(x.ScientificName.ToLower(), keyword) <= 2
                )
            )
            .ToList();

        return products
            .Union(fuzzyMatches)
            .Distinct()
            .ToList();
    }

   
    public async Task<List<Product>> GetByCategoryAsync(int categoryId)
    {
        return await _context.Products
            .Where(p => p.CategoryId == categoryId && !p.IsDeleted)
            .ToListAsync();
    }

  
    public async Task<Product?> GetByBarcodeAsync(string barcode)
    {
        return await _context.Products
            .FirstOrDefaultAsync(p => p.Barcode == barcode && !p.IsDeleted);
    }

    public async Task<bool> IsBarcodeExistsAsync(string barcode)
    {
        return await _context.Products
            .AnyAsync(p => p.Barcode == barcode && !p.IsDeleted);
    }

    
    public async Task<int> GetTotalStockAsync(int productId)
    {
        return await _context.ProductBatches
            .Where(b => b.ProductId == productId)
            .SumAsync(b => (int?)b.Quantity) ?? 0;
    }

    public async Task<bool> IsLowStockAsync(int productId)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(x => x.Id == productId && !x.IsDeleted);

        if (product == null) return false;

        var stock = await GetTotalStockAsync(productId);

        return stock <= product.MinStockLevel;
    }

   
    public async Task<List<Product>> GetLowStockProductsAsync()
    {
        var products = await _context.Products
            .Include(p => p.ProductBatches)
            .Where(p => !p.IsDeleted)
            .ToListAsync();

        return products
            .Where(p =>
                (p.ProductBatches.Sum(b => b.Quantity)) <= p.MinStockLevel)
            .ToList();
    }

   
    public async Task<List<ProductBatch>> GetBatchesAsync(int productId)
    {
        return await _context.ProductBatches
            .Where(b => b.ProductId == productId)
            .OrderBy(b => b.ExpiryDate)
            .ToListAsync();
    }

    public async Task<ProductBatch?> GetOldestBatchAsync(int productId)
    {
        return await _context.ProductBatches
            .Where(b =>
                b.ProductId == productId &&
                b.Quantity > 0 &&
                b.ExpiryDate > DateTime.UtcNow)
            .OrderBy(b => b.ExpiryDate)
            .FirstOrDefaultAsync();
    }

    
    public async Task DeductFromBatchAsync(int productId, int quantity)
    {
        var batches = await _context.ProductBatches
            .Where(b =>
                b.ProductId == productId &&
                b.Quantity > 0)
            .OrderBy(b => b.ExpiryDate)
            .ToListAsync();

        var totalStock = batches.Sum(x => x.Quantity);

        if (totalStock < quantity)
        {
            throw new Exception("Insufficient stock");
        }

        foreach (var batch in batches)
        {
            if (quantity <= 0) break;

            if (batch.Quantity >= quantity)
            {
                batch.Quantity -= quantity;
                quantity = 0;
            }
            else
            {
                quantity -= batch.Quantity;
                batch.Quantity = 0;
            }
        }
    }

    
    public async Task<Product?> GetByIdWithBatchesAsync(int id)
    {
        return await _context.Products
            .Include(p => p.ProductBatches)
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
    }

    public async Task<List<Product>> GetAllWithBatchesAsync()
    {
        return await _context.Products
            .Include(p => p.ProductBatches)
            .Include(p => p.Category)
            .Where(p => !p.IsDeleted)
            .ToListAsync();
    }

    public async Task<bool> AnyAsync(Expression<Func<Product, bool>> predicate)
    {
        return await _context.Products.AnyAsync(predicate);
    }

    
    private int LevenshteinDistance(string source, string target)
    {
        if (string.IsNullOrEmpty(source))
            return target.Length;

        if (string.IsNullOrEmpty(target))
            return source.Length;

        int[,] matrix = new int[source.Length + 1, target.Length + 1];

        for (int i = 0; i <= source.Length; i++)
            matrix[i, 0] = i;

        for (int j = 0; j <= target.Length; j++)
            matrix[0, j] = j;

        for (int i = 1; i <= source.Length; i++)
        {
            for (int j = 1; j <= target.Length; j++)
            {
                int cost = source[i - 1] == target[j - 1] ? 0 : 1;

                matrix[i, j] = Math.Min(
                    Math.Min(
                        matrix[i - 1, j] + 1,
                        matrix[i, j - 1] + 1),
                    matrix[i - 1, j - 1] + cost);
            }
        }

        return matrix[source.Length, target.Length];
    }
}