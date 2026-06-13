public class ScannedProductDto
{
    public int ProductId { get; set; }

    public string ProductName { get; set; }

    public string Barcode { get; set; }

    public decimal Price { get; set; }

    public int BatchId { get; set; }

    public string BatchNumber { get; set; }

    public DateTime ExpiryDate { get; set; }

    public int AvailableQuantity { get; set; }
}