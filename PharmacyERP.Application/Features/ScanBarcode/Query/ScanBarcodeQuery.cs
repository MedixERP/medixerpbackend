using MediatR;
using PharmacyERP.Application.Common.Models;

public class ScanBarcodeQuery : IRequest<Result<ScannedProductDto>>
{
    public string Barcode { get; set; }
}