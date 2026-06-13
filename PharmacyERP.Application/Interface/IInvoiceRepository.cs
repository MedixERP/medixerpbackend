using PharmacyERP.Domain.Entities;

namespace PharmacyERP.Application.Common.Interfaces.Repositories;

public interface IInvoiceRepository : IGenericRepository<Invoice>
{
    Task<Invoice?> GetInvoiceWithItemsAsync(int invoiceId);

    Task<string> GenerateInvoiceNumberAsync();
}