
namespace PharmacyERP.Domain.Entities;

public class InvoiceItem : BaseEntity
{
    public int InvoiceId { get; set; }

    public int ProductId { get; set; }

    public int BatchId { get; set; }

    public int UnitId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal Total { get; set; }

    public Invoice Invoice { get; set; }

    public Product Product { get; set; }

    public ProductBatch Batch { get; set; }

    public Unit Unit { get; set; }
}