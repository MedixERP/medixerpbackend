using MediatR;

public class GetExpiredBatchesQuery
    : IRequest<List<ProductBatchsDto>>
{
}