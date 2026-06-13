using MediatR;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Domain.Entities;

public class ExportLowStockProductsHandler
    : IRequestHandler<ExportLowStockProductsQuery, byte[]>
{
    private readonly IUnitOfWork _uow;
    private readonly IExportService _export;

    public ExportLowStockProductsHandler(
        IUnitOfWork uow,
        IExportService export)
    {
        _uow = uow;
        _export = export;
    }

    public async Task<byte[]> Handle(
        ExportLowStockProductsQuery request,
        CancellationToken cancellationToken)
    {
        var products = await _uow.Repository<Product>()
            .GetAllAsync();

        var data = products
            .Where(x => x.MinStockLevel <= 10)
            .Select(x => new
            {
                x.Name,
                x.Barcode,
                x.MinStockLevel,
                x.SalePrice
            }).ToList();

        if (request.Format.ToLower() == "excel")
            return _export.ExportToExcel(data, "LowStock");

        return _export.ExportToPdf(data, "Low Stock Products");
    }
}