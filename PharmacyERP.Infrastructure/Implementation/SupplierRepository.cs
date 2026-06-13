using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces.Repositories;
using PharmacyERP.Domain.Entities;
using PharmacyERP.Infrastructure.Persistence;

namespace PharmacyERP.Infrastructure.Repositories;

public class SupplierRepository
    : GenericRepository<Supplier>,
      ISupplierRepository
{
    private readonly ApplicationDbContext _context;

    public SupplierRepository(ApplicationDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<bool> IsNameExistsAsync(string name)
    {
        return await _context.Suppliers
            .AnyAsync(x =>
                x.Name == name &&
                !x.IsDeleted);
    }

    public async Task<Supplier?> GetSupplierWithBatchesAsync(int id)
    {
        return await _context.Suppliers
            .Include(x => x.ProductBatches)
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                !x.IsDeleted);
    }
}