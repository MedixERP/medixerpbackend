using MediatR;
using PharmacyERP.Application.Common.Models;

public class ReceivePurchaseOrderCommand : IRequest<Result<MediatR.Unit>>
{
    public int PurchaseOrderId { get; set; }
}