public class InvoiceExportDto
{
    public string InvoiceNumber { get; set; }
    public string CustomerName { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime Date { get; set; }
}