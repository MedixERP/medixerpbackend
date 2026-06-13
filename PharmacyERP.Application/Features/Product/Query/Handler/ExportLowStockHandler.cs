using MediatR;
using PharmacyERP.Application.Common.Interfaces;

public class ExportLowStockHandler
    : IRequestHandler<ExportLowStockQuery, byte[]>
{
    private readonly IUnitOfWork _uow;
    private readonly IExportService _export;

    public ExportLowStockHandler(
        IUnitOfWork uow,
        IExportService export)
    {
        _uow = uow;
        _export = export;
    }

    public async Task<byte[]> Handle(
      ExportLowStockQuery request,
      CancellationToken cancellationToken)
    {
        var products = await _uow.Products
            .GetAllWithBatchesAsync();

        var lowStock = products
            .Where(x =>
                x.ProductBatches.Sum(b => b.Quantity)
                <= x.MinStockLevel)
            .Select(x => new LowStockExportDto
            {
                ProductName = x.Name,
                CurrentStock = x.ProductBatches.Sum(b => b.Quantity),
                MinStockLevel = x.MinStockLevel
            })
            .ToList();

        if (request.Format.ToLower() == "excel")
            return _export.ExportToExcel(lowStock, "LowStock");

        return _export.ExportToPdf(lowStock, "Low Stock Report");
    }
}