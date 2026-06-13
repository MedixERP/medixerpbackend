public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Barcode { get; set; }
    public decimal SalePrice { get; set; }

    public int TotalStock { get; set; }
    public bool IsLowStock { get; set; }

    public List<ProductBatchDto> Batches { get; set; } = new();
}

public class ProductBatchDto
{
    public string BatchNumber { get; set; }
    public int Quantity { get; set; }
    public DateTime ExpiryDate { get; set; }
}