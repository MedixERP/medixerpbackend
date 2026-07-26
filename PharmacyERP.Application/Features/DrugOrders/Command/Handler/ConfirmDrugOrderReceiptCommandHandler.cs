using MediatR;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;
using PharmacyERP.Domain.Enums;

public class ConfirmDrugOrderReceiptCommandHandler
    : IRequestHandler<ConfirmDrugOrderReceiptCommand, Result<string>>
{
    private readonly IUnitOfWork _uow;

    public ConfirmDrugOrderReceiptCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<string>> Handle(
        ConfirmDrugOrderReceiptCommand request,
        CancellationToken cancellationToken)
    {
        var order = await _uow.Repository<DrugOrder>()
            .Query()
            .Include(x => x.Items)
                .ThenInclude(i => i.Product)
                    .ThenInclude(p => p.ProductBatches)
            .FirstOrDefaultAsync(
                x => x.Id == request.OrderId && !x.IsDeleted,
                cancellationToken);

        if (order == null)
            return Result<string>.Failure("Order not found", 404);

        if (order.Status != DrugOrderStatus.Delivered)
            return Result<string>.Failure(
                "Order must be delivered before confirming receipt", 400);

        try
        {
            await _uow.ExecuteInTransactionAsync(async () =>
            {
                foreach (var item in order.Items)
                {
                    var batch = new ProductBatch
                    {
                        ProductId = item.ProductId,
                        BatchNumber = $"DO-{order.OrderNumber}-{item.ProductId}",
                        Quantity = item.Quantity,
                        PurchasePrice = item.UnitPrice,
                        ExpiryDate = DateTime.UtcNow.AddYears(2),
                        ReceivedDate = DateTime.UtcNow,
                        SupplierId = 1,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _uow.Repository<ProductBatch>().AddAsync(batch);

                    var movement = new InventoryMovement
                    {
                        ProductId = item.ProductId,
                        Type = InventoryMovementType.Purchase,
                        Quantity = item.Quantity,
                        BeforeQuantity = item.Product.ProductBatches.Sum(b => b.Quantity),
                        AfterQuantity = item.Product.ProductBatches.Sum(b => b.Quantity) + item.Quantity,
                        Reason = $"Received from drug order {order.OrderNumber}",
                        ReferenceType = "DrugOrder",
                        ReferenceId = order.Id,
                        UserId = order.CreatedByUserId,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _uow.Repository<InventoryMovement>().AddAsync(movement);
                }

                order.Status = DrugOrderStatus.Completed;
                order.ReceivedAt = DateTime.UtcNow;
                order.UpdatedAt = DateTime.UtcNow;

                _uow.Repository<DrugOrder>().Update(order);

                await _uow.SaveChangesAsync(cancellationToken);

            }, cancellationToken);

            return Result<string>.Success(
                "Completed",
                "Order received and stock updated successfully");
        }
        catch (Exception ex)
        {
            return Result<string>.Failure($"An error occurred while confirming receipt: {ex.Message}", 500);
        }
    }
}
