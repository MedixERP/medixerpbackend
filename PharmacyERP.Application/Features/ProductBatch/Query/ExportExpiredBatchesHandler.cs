using MediatR;
using PharmacyERP.Application.Common.Interfaces;

public class ExportExpiredBatchesHandler
    : IRequestHandler<ExportExpiredBatchesQuery, byte[]>
{
    private readonly IUnitOfWork _uow;
    private readonly IExportService _export;

    public ExportExpiredBatchesHandler(
        IUnitOfWork uow,
        IExportService export)
    {
        _uow = uow;
        _export = export;
    }

    public async Task<byte[]> Handle(
        ExportExpiredBatchesQuery request,
        CancellationToken cancellationToken)
    {
        var batches = await _uow.ProductBatches
            .GetExpiredBatchesAsync();

        var data = batches.Select(x => new
        {
            Product = x.Product.Name,
            x.BatchNumber,
            x.Quantity,
            x.ExpiryDate
        }).ToList();

        if (request.Format.ToLower() == "excel")
            return _export.ExportToExcel(data, "ExpiredBatches");

        return _export.ExportToPdf(data, "Expired Batches");
    }
}