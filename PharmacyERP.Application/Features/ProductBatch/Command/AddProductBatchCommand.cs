using MediatR;
using PharmacyERP.Application.Common.Models;

public class AddProductBatchCommand : IRequest<Result<int>>
{
    public int ProductId { get; set; }

    public string BatchNumber { get; set; }

    public int Quantity { get; set; }

    public DateTime ExpiryDate { get; set; }

    public int SupplierId { get; set; }

    public decimal PurchasePrice { get; set; }
}