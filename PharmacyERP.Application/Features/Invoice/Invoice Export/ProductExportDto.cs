public class ProductExportDto
{
    public string Name { get; set; }

    public string Barcode { get; set; }

    public decimal PurchasePrice { get; set; }

    public decimal SalePrice { get; set; }

    public int MinStockLevel { get; set; }

    public bool IsActive { get; set; }
}