using MediatR;
using PharmacyERP.Application.Common.Models;

public class GetLowStockProductsQuery
    : IRequest<Result<List<ProductDto>>>
{
}