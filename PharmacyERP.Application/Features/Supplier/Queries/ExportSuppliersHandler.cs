using MediatR;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Domain.Entities;

public class ExportSuppliersHandler
    : IRequestHandler<ExportSuppliersQuery, byte[]>
{
    private readonly IUnitOfWork _uow;
    private readonly IExportService _export;

    public ExportSuppliersHandler(
        IUnitOfWork uow,
        IExportService export)
    {
        _uow = uow;
        _export = export;
    }

    public async Task<byte[]> Handle(
        ExportSuppliersQuery request,
        CancellationToken cancellationToken)
    {
        var suppliers = await _uow.Repository<Supplier>()
            .GetAllAsync();

        var data = suppliers.Select(x => new
        {
            x.Name,
            x.Phone,
            x.Email,
            x.Address
        }).ToList();

        if (request.Format.ToLower() == "excel")
            return _export.ExportToExcel(data, "Suppliers");

        return _export.ExportToPdf(data, "Suppliers Report");
    }
}