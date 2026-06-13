public class InvoiceDto
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; }
    public string CustomerName { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal FinalAmount { get; set; }
    public bool IsCancelled { get; set; }

    public List<InvoiceItemDto> Items { get; set; }
}

public class InvoiceItemDto
{
    public int ProductId { get; set; }


public int BatchId { get; set; }

    public string ProductName { get; set; }

    public string BatchNumber { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal Total { get; set; }


}
