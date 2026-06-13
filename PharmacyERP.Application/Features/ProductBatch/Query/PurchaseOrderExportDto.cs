public class PurchaseOrderExportDto
{
    public string OrderNumber { get; set; }

    public string SupplierName { get; set; }

    public string Status { get; set; }

    public decimal TotalAmount { get; set; }

    public DateTime CreatedAt { get; set; }
}