using MediatR;

public class UpdateCartQuantityCommand : IRequest<List<PosCartItemDto>>
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}