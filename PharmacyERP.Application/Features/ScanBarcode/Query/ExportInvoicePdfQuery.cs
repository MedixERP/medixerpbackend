using MediatR;

public class ExportInvoicePdfQuery : IRequest<byte[]>
{
    public int InvoiceId { get; set; }
}