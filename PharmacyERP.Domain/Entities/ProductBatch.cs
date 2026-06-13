
namespace PharmacyERP.Domain.Entities;

public class ProductBatch : BaseEntity
{
    public int ProductId { get; set; }

    public string BatchNumber { get; set; }

    public int Quantity { get; set; }

    public DateTime ExpiryDate { get; set; }

    public DateTime ReceivedDate { get; set; }

    public int SupplierId { get; set; }

    public decimal PurchasePrice { get; set; }

    public Product Product { get; set; }

    public Supplier Supplier { get; set; }

    public ICollection<SalesReturnItem> SalesReturnItems { get; set; }
}