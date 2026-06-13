using MediatR;

public class RemoveFromCartCommand : IRequest<List<PosCartItemDto>>
{
    public int ProductId { get; set; }
}