using MediatR;
using PharmacyERP.Application.Common.Models;

public class UpdateProductCommand : IRequest<Result<string>>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal SalePrice { get; set; }
    public int MinStockLevel { get; set; }
}