using MediatR;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Domain.Entities;

public class ExportInvoicesHandler
    : IRequestHandler<ExportInvoicesQuery, byte[]>
{
    private readonly IUnitOfWork _uow;
    private readonly IExportService _export;

    public ExportInvoicesHandler(
        IUnitOfWork uow,
        IExportService export)
    {
        _uow = uow;
        _export = export;
    }

    public async Task<byte[]> Handle(
        ExportInvoicesQuery request,
        CancellationToken cancellationToken)
    {
        var invoices = await _uow.Repository<Invoice>()
            .Query()
            .Include(x => x.Customer)
            .ToListAsync(cancellationToken);

        var data = invoices.Select(x => new InvoiceExportDto
        {
            InvoiceNumber = x.InvoiceNumber,
            CustomerName = x.Customer != null
                ? x.Customer.FullName
                : "Walk In Customer",

            TotalAmount = x.FinalAmount,
            Date = x.CreatedAt
        }).ToList();

        if (request.Format.ToLower() == "excel")
            return _export.ExportToExcel(data, "Invoices");

        return _export.ExportToPdf(data, "Invoices Report");
    }
}