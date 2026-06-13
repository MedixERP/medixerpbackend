public class CheckoutResponseDto
{
    public int InvoiceId { get; set; }
    public string InvoiceNumber { get; set; }
    public decimal Total { get; set; }
    public decimal FinalTotal { get; set; }
    public decimal Change { get; set; }
    public string PdfUrl { get; set; }
}