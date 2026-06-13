using System.Collections.Concurrent;

public class CartService : ICartService
{
    private static readonly ConcurrentDictionary<string, List<PosCartItemDto>> _userCarts
        = new();

    public void AddItem(string userId, PosCartItemDto item)
    {
        var cart = _userCarts.GetOrAdd(userId, _ => new List<PosCartItemDto>());

        var existing = cart.FirstOrDefault(x =>
            x.ProductId == item.ProductId &&
            x.BatchId == item.BatchId);

        if (existing != null)
        {
            existing.Quantity += item.Quantity;
        }
        else
        {
            cart.Add(item);
        }
    }

    public List<PosCartItemDto> GetCart(string userId)
    {
        return _userCarts.TryGetValue(userId, out var cart)
            ? cart
            : new List<PosCartItemDto>();
    }

    public void UpdateItem(string userId, int productId, int quantity)
    {
        var cart = _userCarts.GetOrAdd(userId, _ => new List<PosCartItemDto>());

        var item = cart.FirstOrDefault(x => x.ProductId == productId);

        if (item != null)
            item.Quantity = quantity;
    }

    public void RemoveItem(string userId, int productId)
    {
        if (_userCarts.TryGetValue(userId, out var cart))
        {
            var item = cart.FirstOrDefault(x => x.ProductId == productId);
            if (item != null)
                cart.Remove(item);
        }
    }

    public void Clear(string userId)
    {
        _userCarts[userId] = new List<PosCartItemDto>();
    }
}