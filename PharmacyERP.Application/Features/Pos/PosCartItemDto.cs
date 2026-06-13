public class PosCartItemDto
{
    public int ProductId { get; set; }
    public int BatchId { get; set; }

    public string ProductName { get; set; }
    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public decimal Total => UnitPrice * Quantity;
}