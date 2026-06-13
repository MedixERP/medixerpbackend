using MediatR;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Domain.Entities;

public class ExportCustomersHandler
    : IRequestHandler<ExportCustomersQuery, byte[]>
{
    private readonly IUnitOfWork _uow;
    private readonly IExportService _export;

    public ExportCustomersHandler(
        IUnitOfWork uow,
        IExportService export)
    {
        _uow = uow;
        _export = export;
    }

    public async Task<byte[]> Handle(
        ExportCustomersQuery request,
        CancellationToken cancellationToken)
    {
        var customers = await _uow.Repository<Customer>()
            .GetAllAsync();

        var data = customers.Select(x => new
        {
            x.FullName,
            x.Phone,
            x.Address,
            x.CreatedAt
        }).ToList();

        if (request.Format.ToLower() == "excel")
            return _export.ExportToExcel(data, "Customers");

        return _export.ExportToPdf(data, "Customers Report");
    }
}