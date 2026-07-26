namespace PharmacyERP.Domain.Entities;

public class SupplierPayment : BaseEntity
{
    public int SupplierId { get; set; }
    public int PurchaseOrderId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; }
    public DateTime PaidAt { get; set; }
    public int CreatedByUserId { get; set; }
    public Supplier Supplier { get; set; }
    public PurchaseOrder PurchaseOrder { get; set; }
    public ApplicationUser CreatedByUser { get; set; }
}