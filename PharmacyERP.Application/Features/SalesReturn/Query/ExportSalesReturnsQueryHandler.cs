using MediatR;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Domain.Entities;

public class ExportSalesReturnsQueryHandler
: IRequestHandler<ExportSalesReturnsQuery, byte[]>
{
    private readonly IUnitOfWork _uow;
    private readonly IExportService _export;

public ExportSalesReturnsQueryHandler(
    IUnitOfWork uow,
    IExportService export)
    {
        _uow = uow;
        _export = export;
    }

    public async Task<byte[]> Handle(
        ExportSalesReturnsQuery request,
        CancellationToken cancellationToken)
    {
        var data = await _uow.Repository<SalesReturn>()
            .Query()
            .Include(x => x.Invoice)
            .Select(x => new
            {
                x.Id,
                x.Invoice.InvoiceNumber,
                x.Reason,
                x.TotalAmount,
                x.CreatedAt
            })
            .ToListAsync(cancellationToken);

        if (request.Format.ToLower() == "excel")
            return _export.ExportToExcel(data, "Sales Returns");

        return _export.ExportToPdf(data, "Sales Returns Report");
    }

}
