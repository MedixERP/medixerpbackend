
namespace PharmacyERP.Domain.Entities;

public class SalesReturnItem : BaseEntity
{
    public int SalesReturnId { get; set; }

    public int ProductId { get; set; }

    public int BatchId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal Total { get; set; }

    public SalesReturn SalesReturn { get; set; }

    public Product Product { get; set; }

    public ProductBatch Batch { get; set; }
}