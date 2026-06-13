using MediatR;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;

public class GetPurchaseOrderByIdQueryHandler
    : IRequestHandler<GetPurchaseOrderByIdQuery, Result<PurchaseOrderDto>>
{
    private readonly IUnitOfWork _uow;

    public GetPurchaseOrderByIdQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<PurchaseOrderDto>> Handle(
        GetPurchaseOrderByIdQuery request,
        CancellationToken cancellationToken)
    {
        var order = await _uow.Repository<PurchaseOrder>()
            .Query()
            .AsNoTracking()
            .Include(x => x.Supplier)
            .Include(x => x.PurchaseOrderItems)
            .FirstOrDefaultAsync(
                x => x.Id == request.Id,
                cancellationToken);

        if (order == null)
        {
            return Result<PurchaseOrderDto>.Failure(
                "Purchase order not found",
                404);
        }

        var dto = new PurchaseOrderDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            SupplierId = order.SupplierId,
            SupplierName = order.Supplier?.Name ?? string.Empty,
            Status = order.Status.ToString(),
            CreatedAt = order.CreatedAt,

            ItemsCount = order.PurchaseOrderItems?.Count ?? 0,

            TotalAmount = order.PurchaseOrderItems != null
                ? order.PurchaseOrderItems.Sum(x =>
                    x.Quantity * x.UnitPrice)
                : 0
        };

        return Result<PurchaseOrderDto>.Success(
            dto,
            "Purchase order retrieved successfully");
    }
}