using MediatR;
using PharmacyERP.Application.Common.Interfaces;

public class ExportExpiredProductsHandler
    : IRequestHandler<ExportExpiredProductsQuery, byte[]>
{
    private readonly IUnitOfWork _uow;
    private readonly IExportService _export;

    public ExportExpiredProductsHandler(
        IUnitOfWork uow,
        IExportService export)
    {
        _uow = uow;
        _export = export;
    }

    public async Task<byte[]> Handle(
        ExportExpiredProductsQuery request,
        CancellationToken cancellationToken)
    {
        var batches = await _uow.ProductBatches
            .GetExpiredBatchesAsync();

        var data = batches.Select(x => new ExpiredProductExportDto
        {
            ProductName = x.Product.Name,
            BatchNumber = x.BatchNumber,
            Quantity = x.Quantity,
            ExpiryDate = x.ExpiryDate
        }).ToList();

        if (request.Format.ToLower() == "excel")
            return _export.ExportToExcel(data, "ExpiredProducts");

        return _export.ExportToPdf(data, "Expired Products Report");
    }
}