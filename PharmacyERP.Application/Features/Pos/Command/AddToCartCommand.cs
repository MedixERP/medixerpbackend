using MediatR;
using PharmacyERP.Application.Common.Models;

public class AddToCartCommand : IRequest<Result<List<PosCartItemDto>>>
{
    public string Barcode { get; set; }
    public int Quantity { get; set; }
}