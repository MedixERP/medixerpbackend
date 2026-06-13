public class CustomerPurchaseHistoryDto
{
    public int CustomerId { get; set; }

    public string CustomerName { get; set; }

    public List<InvoiceHistoryDto> Invoices { get; set; }
}

public class InvoiceHistoryDto
{
    public string InvoiceNumber { get; set; }

    public decimal FinalAmount { get; set; }

    public DateTime CreatedAt { get; set; }
}