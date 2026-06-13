using PharmacyERP.Domain.Entities;

namespace PharmacyERP.Application.Common.Interfaces;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<bool> IsNameExistsAsync(string name);

    Task<Category?> GetByIdWithProductsAsync(int id);
}