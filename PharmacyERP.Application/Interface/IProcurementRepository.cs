using PharmacyERP.Domain.Entities;

public interface IProcurementRepository
{
    Task<PurchaseOrder?> GetByIdAsync(int id);
    Task<List<PurchaseOrder>> GetAllAsync();

    Task AddAsync(PurchaseOrder order);
    void Update(PurchaseOrder order);
}