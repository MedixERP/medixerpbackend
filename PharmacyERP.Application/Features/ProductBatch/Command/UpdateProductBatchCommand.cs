using MediatR;
using PharmacyERP.Application.Common.Models;

public class UpdateProductBatchCommand : IRequest<Result<MediatR.Unit>>
{
    public int Id { get; set; }

    public int Quantity { get; set; }

    public DateTime ExpiryDate { get; set; }

    public decimal PurchasePrice { get; set; }
}