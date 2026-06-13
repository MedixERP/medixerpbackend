
namespace PharmacyERP.Domain.Entities;

public class Supplier : BaseEntity
{
    public string Name { get; set; }

    public string Phone { get; set; } = null!;
    public string Email { get; set; }

    public string Address { get; set; }

    public ICollection<ProductBatch> ProductBatches { get; set; }

    public ICollection<PurchaseOrder> PurchaseOrders { get; set; }
}