using MediatR;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Domain.Entities;

public class ExportProductsHandler
    : IRequestHandler<ExportProductsQuery, byte[]>
{
    private readonly IUnitOfWork _uow;
    private readonly IExportService _export;

    public ExportProductsHandler(
        IUnitOfWork uow,
        IExportService export)
    {
        _uow = uow;
        _export = export;
    }

    public async Task<byte[]> Handle(
        ExportProductsQuery request,
        CancellationToken cancellationToken)
    {
        var products = await _uow.Repository<Product>()
            .GetAllAsync();

        var data = products.Select(x => new ProductExportDto
        {
            Name = x.Name,
            Barcode = x.Barcode,
            PurchasePrice = x.PurchasePrice,
            SalePrice = x.SalePrice,
            MinStockLevel = x.MinStockLevel,
            IsActive = x.IsActive
        }).ToList();

        if (request.Format.ToLower() == "excel")
        {
            return _export.ExportToExcel(
                data,
                "Products");
        }

        return _export.ExportToPdf(
            data,
            "Products Report");
    }
}