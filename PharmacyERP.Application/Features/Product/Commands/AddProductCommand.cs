using MediatR;
using PharmacyERP.Application.Common.Models;

public class AddProductCommand : IRequest<Result<int>>
{
    public string Name { get; set; }
    public string ScientificName { get; set; }

    public int CategoryId { get; set; }

    public decimal PurchasePrice { get; set; }
    public decimal SalePrice { get; set; }

    public int MinStockLevel { get; set; }
}