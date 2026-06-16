using Microsoft.EntityFrameworkCore.Storage;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Interfaces.Repositories;

public interface IUnitOfWork
{
    IProductRepository Products { get; }
    ICategoryRepository Categories { get; }
    IInvoiceRepository Invoices { get; }
    ICustomerRepository Customers { get; }
    ISupplierRepository Suppliers { get; }
    IProcurementRepository Procurement { get; }
    IProductBatchRepository ProductBatches { get; }

    IDashboardRepository Dashboard { get; }

    IExportService Export { get; }
    IUserSettingsRepository UserSettings { get; }


    IGenericRepository<T> Repository<T>() where T : class;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<IDbContextTransaction> BeginTransactionAsync();

}