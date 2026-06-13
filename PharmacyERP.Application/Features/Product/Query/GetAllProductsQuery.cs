using MediatR;
using PharmacyERP.Application.Common.Models;

public class GetAllProductsQuery
    : ProductFilterRequest,
      IRequest<Result<PaginatedResult<ProductDto>>>
{
}