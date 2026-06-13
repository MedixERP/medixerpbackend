using PharmacyERP.Domain.Entities;

namespace PharmacyERP.Application.Common.Interfaces.Repositories;

public interface ICustomerRepository : IGenericRepository<Customer>
{
    Task<bool> IsPhoneExistsAsync(string phone);

    Task<Customer?> GetCustomerWithInvoicesAsync(int customerId);
}