using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces.Repositories;
using PharmacyERP.Domain.Entities;
using PharmacyERP.Infrastructure.Persistence;

public class CustomerRepository
    : GenericRepository<Customer>, ICustomerRepository
{
    private readonly ApplicationDbContext _context;

    public CustomerRepository(ApplicationDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<bool> IsPhoneExistsAsync(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return false;

        phone = phone.Trim();

        return await _context.Customers
            .AnyAsync(x =>
                !x.IsDeleted &&
                x.Phone == phone);
    }

    public async Task<Customer?> GetCustomerWithInvoicesAsync(int customerId)
    {
        return await _context.Customers
            .Include(x => x.Invoices)
                .ThenInclude(i => i.InvoiceItems)
            .FirstOrDefaultAsync(x =>
                x.Id == customerId &&
                !x.IsDeleted);
    }
}