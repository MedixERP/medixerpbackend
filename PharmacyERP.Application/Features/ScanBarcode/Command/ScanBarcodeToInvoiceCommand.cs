using MediatR;
using PharmacyERP.Application.Common.Models;

public class ScanBarcodeToInvoiceCommand : IRequest<Result<int>>
{
    public int InvoiceId { get; set; }
    public string Barcode { get; set; }
    public int Quantity { get; set; }
}