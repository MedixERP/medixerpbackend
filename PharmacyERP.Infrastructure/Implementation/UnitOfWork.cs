using Microsoft.EntityFrameworkCore;
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

    public IUserSettingsRepository UserSettings { get; } 

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
        IExportService export,
        IUserSettingsRepository userSettings) 
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

        UserSettings = userSettings; 
    }

    public IGenericRepository<T> Repository<T>() where T : class
        => new GenericRepository<T>(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);

    public async Task<IDbContextTransaction> BeginTransactionAsync()
        => await _context.Database.BeginTransactionAsync();

    public async Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken cancellationToken = default)
    {
        var strategy = _context.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                await action();
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }
}