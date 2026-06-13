public class SalesReturnDto
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public string InvoiceNumber { get; set; }
    public string ReturnedBy { get; set; }
    public string Reason { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }

public List<SalesReturnItemDto> Items { get; set; }


}

public class SalesReturnItemDto
{
    public string ProductName { get; set; }
    public string BatchNumber { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Total { get; set; }
}
