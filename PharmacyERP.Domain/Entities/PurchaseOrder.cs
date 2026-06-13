using PharmacyERP.Domain.Enums;
namespace PharmacyERP.Domain.Entities;

public class PurchaseOrder : BaseEntity
{
    public string OrderNumber { get; set; }

    public int SupplierId { get; set; }

    public int CreatedByUserId { get; set; }

    public PurchaseOrderStatus Status { get; set; }

    public decimal TotalAmount { get; set; }

    public Supplier Supplier { get; set; }

    public ApplicationUser CreatedByUser { get; set; }

    public ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; }

    public ICollection<GoodsReceipt> GoodsReceipts { get; set; }
}