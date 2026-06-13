using MediatR;
using PharmacyERP.Application.Common.Models;

public class CreatePurchaseOrderCommand : IRequest<Result<int>>
{
    public int SupplierId { get; set; }
    public List<CreatePurchaseItemDto> Items { get; set; }
}

public class CreatePurchaseItemDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}