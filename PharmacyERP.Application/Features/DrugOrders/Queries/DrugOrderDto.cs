using PharmacyERP.Domain.Enums;

public class DrugOrderDto
{
    public int Id { get; set; }
    public string OrderNumber { get; set; }
    public string CompanyName { get; set; }
    public string CreatedBy { get; set; }
    public string Status { get; set; }
    public string? RejectionReason { get; set; }
    public string? SupplierName { get; set; }
    public string? SupplierPhone { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime? ReceivedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<DrugOrderItemDto> Items { get; set; } = new();
}

public class DrugOrderItemDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Total { get; set; }
}

public class CreateDrugOrderItemDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}