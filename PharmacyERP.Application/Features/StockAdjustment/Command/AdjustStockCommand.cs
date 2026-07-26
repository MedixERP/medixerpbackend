using MediatR;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Enums;

public class AdjustStockCommand : IRequest<Result<StockAdjustmentDto>>
{
    public int BatchId { get; set; }
    public int NewQuantity { get; set; }
    public AdjustmentReasonType ReasonType { get; set; } 
    public string? Notes { get; set; } 
}