using System.Collections.Concurrent;

public interface ICartService
{
    List<PosCartItemDto> GetCart(string userId);
    void AddItem(string userId, PosCartItemDto item);
    void UpdateItem(string userId, int productId, int quantity);
    void RemoveItem(string userId, int productId);
    void Clear(string userId);
}