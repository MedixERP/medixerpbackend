using MediatR;
using PharmacyERP.Application.Common.Models;

namespace PharmacyERP.Application.Features.Product.Query;

public class GetProductByIdWithBatchesQuery
    : IRequest<Result<ProductDto>>
{
    public int Id { get; set; }
}