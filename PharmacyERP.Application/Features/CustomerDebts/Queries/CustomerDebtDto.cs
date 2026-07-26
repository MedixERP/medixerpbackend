public class CustomerDebtDto
{
    public int CustomerId { get; set; }
    public int InvoiceId { get; set; }
    public string InvoiceNumber { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal Remaining { get; set; }
}