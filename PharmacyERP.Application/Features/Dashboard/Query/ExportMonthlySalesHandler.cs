using MediatR;
using PharmacyERP.Application.Common.Interfaces;

public class ExportMonthlySalesHandler
    : IRequestHandler<ExportMonthlySalesQuery, byte[]>
{
    private readonly IUnitOfWork _uow;
    private readonly IExportService _export;

    public ExportMonthlySalesHandler(
        IUnitOfWork uow,
        IExportService export)
    {
        _uow = uow;
        _export = export;
    }

    public async Task<byte[]> Handle(
        ExportMonthlySalesQuery request,
        CancellationToken cancellationToken)
    {
        var sales = await _uow.Dashboard
            .GetMonthlySalesAsync();

        var data = sales.Select(x => new MonthlySalesExportDto
        {
            Month = x.Month,
            TotalSales = x.Sales
        }).ToList();

        if (request.Format.ToLower() == "excel")
            return _export.ExportToExcel(data, "MonthlySales");

        return _export.ExportToPdf(data, "Monthly Sales Report");
    }
}