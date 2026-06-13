using MediatR;

public class GetTopSellingQuery : IRequest<List<TopSellingProductDto>>
{
    public int Count { get; set; } = 5;
}