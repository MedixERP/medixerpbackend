using MediatR;
using PharmacyERP.Application.Common.Models;

public class CheckoutCommand : IRequest<Result<CheckoutResponseDto>>
{
    public int? CustomerId { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal Discount { get; set; } = 0;
}