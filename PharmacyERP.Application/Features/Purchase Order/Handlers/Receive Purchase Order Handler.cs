using MediatR;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;
using PharmacyERP.Domain.Enums;

public class ReceivePurchaseOrderCommandHandler
    : IRequestHandler<ReceivePurchaseOrderCommand, Result<MediatR.Unit>>
{
    private readonly IUnitOfWork _uow;

    public ReceivePurchaseOrderCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<MediatR.Unit>> Handle(
    ReceivePurchaseOrderCommand request,
    CancellationToken cancellationToken)
    {
        var order = await _uow.Repository<PurchaseOrder>()
            .Query()
            .Include(x => x.PurchaseOrderItems)
            .FirstOrDefaultAsync(
                x => x.Id == request.PurchaseOrderId,
                cancellationToken);

        if (order == null)
            return Result<MediatR.Unit>.Failure("Order not found", 404);

        if (order.Status == PurchaseOrderStatus.Received)
            return Result<MediatR.Unit>.Failure("Already received", 400);

        var supplierExists = await _uow.Suppliers
            .Query()
            .AnyAsync(x => x.Id == order.SupplierId);

        if (!supplierExists)
            return Result<MediatR.Unit>.Failure("Supplier not found", 404);

        foreach (var item in order.PurchaseOrderItems)
        {
            if (item.Quantity <= 0)
                return Result<MediatR.Unit>.Failure("Invalid quantity in order", 400);

            var product = await _uow.Products
                .GetByIdAsync(item.ProductId);

            if (product == null)
                return Result<MediatR.Unit>.Failure("Product not found", 404);

            var batch = new ProductBatch
            {
                ProductId = item.ProductId,

                SupplierId = order.SupplierId,

                BatchNumber = $"PO-{order.Id}-{item.Id}",
                Quantity = item.Quantity,
                PurchasePrice = item.UnitPrice,
                ExpiryDate = DateTime.UtcNow.AddMonths(12),
                ReceivedDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await _uow.ProductBatches.AddAsync(batch);
        }

        order.Status = PurchaseOrderStatus.Received;
        order.UpdatedAt = DateTime.UtcNow;

        _uow.Repository<PurchaseOrder>().Update(order);

        await _uow.SaveChangesAsync(cancellationToken);

        return Result<MediatR.Unit>
            .Success(MediatR.Unit.Value, "Order received successfully");
    }
}