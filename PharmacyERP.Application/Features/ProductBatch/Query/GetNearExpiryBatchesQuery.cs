using MediatR;

public class GetNearExpiryBatchesQuery
    : IRequest<List<ProductBatchsDto>>
{
    public int Days { get; set; } = 30;
}