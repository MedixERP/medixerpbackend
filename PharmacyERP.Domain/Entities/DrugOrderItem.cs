namespace PharmacyERP.Domain.Entities;

public class DrugOrderItem : BaseEntity
{
    public int DrugOrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Total { get; set; }
    public DrugOrder DrugOrder { get; set; }
    public Product Product { get; set; }
}