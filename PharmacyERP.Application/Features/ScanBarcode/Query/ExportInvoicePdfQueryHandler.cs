using MediatR;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Domain.Entities;

public class ExportInvoicePdfQueryHandler
    : IRequestHandler<ExportInvoicePdfQuery, byte[]>
{
    private readonly IUnitOfWork _uow;
    private readonly IExportService _export;

    public ExportInvoicePdfQueryHandler(
        IUnitOfWork uow,
        IExportService export)
    {
        _uow = uow;
        _export = export;
    }

    public async Task<byte[]> Handle(
        ExportInvoicePdfQuery request,
        CancellationToken cancellationToken)
    {
        var invoice = await _uow.Repository<Invoice>()
            .Query()
            .Include(x => x.Customer)
            .Include(x => x.InvoiceItems)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(
                x => x.Id == request.InvoiceId,
                cancellationToken);

        if (invoice == null)
            throw new Exception("Invoice not found");

        var data = invoice.InvoiceItems.Select(x => new
        {
            Product = x.Product.Name,
            Quantity = x.Quantity,
            Price = x.UnitPrice,
            Total = x.Total
        }).ToList();

        return _export.ExportToPdf(
            data,
            $"Invoice #{invoice.InvoiceNumber}");
    }
}