
namespace PharmacyERP.Domain.Entities;

public class PurchaseOrderItem : BaseEntity
{
    public int PurchaseOrderId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal Total => Quantity * UnitPrice;

    public PurchaseOrder PurchaseOrder { get; set; }

    public Product Product { get; set; }
}