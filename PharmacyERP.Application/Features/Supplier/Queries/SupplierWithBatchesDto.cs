public class SupplierWithBatchesDto
{
    public int Id { get; set; }
    public string Name { get; set; }

    public List<ProductBatchDto> Batches { get; set; }
}

public class productBatchDto
{
    public string BatchNumber { get; set; }
    public int Quantity { get; set; }
    public DateTime ExpiryDate { get; set; }
}