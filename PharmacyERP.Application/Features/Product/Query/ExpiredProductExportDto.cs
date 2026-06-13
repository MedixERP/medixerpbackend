public class ExpiredProductExportDto
{
    public string ProductName { get; set; }

    public string BatchNumber { get; set; }

    public int Quantity { get; set; }

    public DateTime ExpiryDate { get; set; }
}