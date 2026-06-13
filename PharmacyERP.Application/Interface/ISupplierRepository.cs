using PharmacyERP.Domain.Entities;

namespace PharmacyERP.Application.Common.Interfaces.Repositories;

public interface ISupplierRepository
    : IGenericRepository<Supplier>
{
    Task<bool> IsNameExistsAsync(string name);

    Task<Supplier?> GetSupplierWithBatchesAsync(int id);
}