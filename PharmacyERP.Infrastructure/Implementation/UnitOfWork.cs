using Microsoft.EntityFrameworkCore.Storage;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Interfaces.Repositories;
using PharmacyERP.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public IProductRepository Products { get; }
    public ICategoryRepository Categories { get; }
    public IInvoiceRepository Invoices { get; }
    public ICustomerRepository Customers { get; }
    public ISupplierRepository Suppliers { get; }
    public IProcurementRepository Procurement { get; }
    public IProductBatchRepository ProductBatches { get; }

    public IDashboardRepository Dashboard { get; }

    public IExportService Export { get; }

    public UnitOfWork(
        ApplicationDbContext context,
        IProductRepository products,
        ICategoryRepository categories,
        IInvoiceRepository invoices,
        ICustomerRepository customers,
        ISupplierRepository suppliers,
        IProcurementRepository procurement,
        IProductBatchRepository productBatches,
        IDashboardRepository dashboard,
        IExportService export)
    {
        _context = context;

        Products = products;
        Categories = categories;
        Invoices = invoices;
        Customers = customers;
        Suppliers = suppliers;
        Procurement = procurement;
        ProductBatches = productBatches;
        Dashboard = dashboard;

        Export = export;
    }

    public IGenericRepository<T> Repository<T>() where T : class
        => new GenericRepository<T>(_context);

    public async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _context.Database.BeginTransactionAsync();
    }
}