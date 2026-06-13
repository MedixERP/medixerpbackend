public class ProductBatchsDto
{
    public int Id { get; set; }

    public string BatchNumber { get; set; }

    public int Quantity { get; set; }

    public DateTime ExpiryDate { get; set; }

    public decimal PurchasePrice { get; set; }

    public bool IsExpired { get; set; }

    public int ProductId { get; set; }

    public string ProductName { get; set; }
}