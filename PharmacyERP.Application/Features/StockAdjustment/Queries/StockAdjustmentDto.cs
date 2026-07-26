public class StockAdjustmentDto
{
    public int MovementId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public int BatchId { get; set; }
    public string BatchNumber { get; set; }
    public int BeforeQuantity { get; set; }
    public int AfterQuantity { get; set; }
    public int QuantityChanged { get; set; }
    public string Reason { get; set; } 
    public DateTime CreatedAt { get; set; }
}