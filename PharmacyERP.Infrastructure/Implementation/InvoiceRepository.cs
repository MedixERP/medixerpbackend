using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces.Repositories;
using PharmacyERP.Domain.Entities;
using PharmacyERP.Infrastructure.Persistence;

namespace PharmacyERP.Infrastructure.Repositories;

public class InvoiceRepository
    : GenericRepository<Invoice>, IInvoiceRepository
{
    private readonly ApplicationDbContext _context;

    public InvoiceRepository(ApplicationDbContext context)
        : base(context)
    {
        _context = context;
    }

 
    public async Task<Invoice?> GetInvoiceWithItemsAsync(int invoiceId)
    {
        return await _context.Invoices
            .AsNoTracking()
            .Include(i => i.Customer)
            .Include(i => i.InvoiceItems)
                .ThenInclude(ii => ii.Product)
            .Include(i => i.InvoiceItems)
                .ThenInclude(ii => ii.Batch)
            .FirstOrDefaultAsync(i =>
                i.Id == invoiceId &&
                !i.IsDeleted);
    }

    
    public async Task<string> GenerateInvoiceNumberAsync()
    {
        var lastInvoice = await _context.Invoices
            .OrderByDescending(i => i.Id)
            .Select(i => new { i.Id })
            .FirstOrDefaultAsync();

        var nextId = (lastInvoice?.Id ?? 0) + 1;

        return $"INV-{nextId:D5}";
    }
}