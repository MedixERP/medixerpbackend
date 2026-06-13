using MediatR;
using PharmacyERP.Application.Common.Models;

public class GetPurchaseOrderByIdQuery
    : IRequest<Result<PurchaseOrderDto>>
{
    public int Id { get; set; }
}