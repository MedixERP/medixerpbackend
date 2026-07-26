using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;
using PharmacyERP.Domain.Enums;
using System.Security.Claims;

public class AdjustStockCommandHandler
    : IRequestHandler<AdjustStockCommand, Result<StockAdjustmentDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IHttpContextAccessor _http;

    public AdjustStockCommandHandler(IUnitOfWork uow, IHttpContextAccessor http)
    {
        _uow = uow;
        _http = http;
    }

    public async Task<Result<StockAdjustmentDto>> Handle(
        AdjustStockCommand request,
        CancellationToken cancellationToken)
    {
        var batch = await _uow.Repository<ProductBatch>()
            .Query()
            .Include(x => x.Product)
            .FirstOrDefaultAsync(
                x => x.Id == request.BatchId && !x.IsDeleted,
                cancellationToken);

        if (batch == null)
            return Result<StockAdjustmentDto>.Failure("Batch not found", 404);

        if (batch.Quantity == request.NewQuantity)
            return Result<StockAdjustmentDto>.Failure(
                "New quantity must differ from current quantity", 400);

        var userIdClaim = _http.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userId = userIdClaim != null ? int.Parse(userIdClaim) : 0;

        var beforeQuantity = batch.Quantity;
        var afterQuantity = request.NewQuantity;

        batch.Quantity = afterQuantity;
        batch.UpdatedAt = DateTime.UtcNow;

        _uow.ProductBatches.Update(batch);

        string dynamicReason = request.ReasonType switch
        {
            AdjustmentReasonType.Broken => "Broken",
            AdjustmentReasonType.Damaged => "Damaged",
            AdjustmentReasonType.Lost => "Lost",
            _ => "Inventory Audit / Other"
        };

        if (!string.IsNullOrWhiteSpace(request.Notes))
        {
            dynamicReason += $" ({request.Notes.Trim()})";
        }

        var movement = new InventoryMovement
        {
            ProductId = batch.ProductId,
            BatchId = batch.Id,
            Type = InventoryMovementType.Adjustment,
            Quantity = Math.Abs(afterQuantity - beforeQuantity),
            BeforeQuantity = beforeQuantity,
            AfterQuantity = afterQuantity,
            Reason = dynamicReason,
            ReferenceType = "StockAdjustment",
            ReferenceId = batch.Id,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.Repository<InventoryMovement>().AddAsync(movement);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result<StockAdjustmentDto>.Success(
            new StockAdjustmentDto
            {
                MovementId = movement.Id,
                ProductId = batch.ProductId,
                ProductName = batch.Product?.Name ?? "",
                BatchId = batch.Id,
                BatchNumber = batch.BatchNumber,
                BeforeQuantity = beforeQuantity,
                AfterQuantity = afterQuantity,
                QuantityChanged = afterQuantity - beforeQuantity,
                Reason = movement.Reason,
                CreatedAt = movement.CreatedAt
            },
            "Stock adjusted successfully");
    }
}