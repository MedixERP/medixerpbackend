using MediatR;
using PharmacyERP.Application.Common.Models;

public class GetCustomerPurchaseHistoryQuery
    : IRequest<Result<CustomerPurchaseHistoryDto>>
{
    public int CustomerId { get; set; }
}