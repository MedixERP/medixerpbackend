using MediatR;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Domain.Entities;

public class ExportPurchaseOrdersHandler
    : IRequestHandler<ExportPurchaseOrdersQuery, byte[]>
{
    private readonly IUnitOfWork _uow;
    private readonly IExportService _export;

    public ExportPurchaseOrdersHandler(
        IUnitOfWork uow,
        IExportService export)
    {
        _uow = uow;
        _export = export;
    }

    public async Task<byte[]> Handle(
        ExportPurchaseOrdersQuery request,
        CancellationToken cancellationToken)
    {
        var orders = await _uow.Repository<PurchaseOrder>()
            .Query()
            .Include(x => x.Supplier)
            .Include(x => x.PurchaseOrderItems)
            .ToListAsync(cancellationToken);

        var data = orders.Select(x => new PurchaseOrderExportDto
        {
            OrderNumber = x.OrderNumber,
            SupplierName = x.Supplier.Name,
            Status = x.Status.ToString(),
            TotalAmount = x.PurchaseOrderItems.Sum(i =>
                i.Quantity * i.UnitPrice),
            CreatedAt = x.CreatedAt
        }).ToList();

        if (request.Format.ToLower() == "excel")
            return _export.ExportToExcel(data, "PurchaseOrders");

        return _export.ExportToPdf(data, "Purchase Orders Report");
    }
}