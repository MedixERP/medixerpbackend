using Microsoft.EntityFrameworkCore;
using PharmacyERP.Domain.Entities;
using PharmacyERP.Infrastructure.Persistence;

public class ProcurementRepository : IProcurementRepository
{
    private readonly ApplicationDbContext _context;

    public ProcurementRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PurchaseOrder?> GetByIdAsync(int id)
    {
        return await _context.PurchaseOrders
            .AsNoTracking()
            .Include(x => x.Supplier)
            .Include(x => x.PurchaseOrderItems)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<PurchaseOrder>> GetAllAsync()
    {
        return await _context.PurchaseOrders
            .AsNoTracking()
            .Include(x => x.Supplier)
            .Include(x => x.PurchaseOrderItems)
            .ToListAsync();
    }
    public async Task AddAsync(PurchaseOrder order)
    {
        await _context.PurchaseOrders.AddAsync(order);
    }

    public void Update(PurchaseOrder order)
    {
        _context.PurchaseOrders.Update(order);
    }
}