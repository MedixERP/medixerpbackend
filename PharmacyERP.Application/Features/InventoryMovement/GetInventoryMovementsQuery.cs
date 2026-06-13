using MediatR;
using PharmacyERP.Application.Common.Models;

public class GetInventoryMovementsQuery
    : PaginationRequest,
      IRequest<Result<PaginatedResult<InventoryMovementDto>>>
{
    public int? ProductId { get; set; }
    public string? Type { get; set; }
}