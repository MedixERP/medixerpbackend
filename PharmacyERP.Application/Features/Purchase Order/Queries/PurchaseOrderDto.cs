public class PurchaseOrderDto
{
    public int Id { get; set; }

    public string OrderNumber { get; set; }

    public int SupplierId { get; set; }

    public string SupplierName { get; set; }

    public string Status { get; set; }

    public decimal TotalAmount { get; set; }

    public int ItemsCount { get; set; }

    public DateTime CreatedAt { get; set; }
}