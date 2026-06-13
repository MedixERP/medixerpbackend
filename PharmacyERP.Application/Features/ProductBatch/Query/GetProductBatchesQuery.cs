using MediatR;
using PharmacyERP.Application.Common.Models;

public class GetProductBatchesQuery
    : IRequest<Result<List<ProductBatchsDto>>>
{
    public int ProductId { get; set; }
}