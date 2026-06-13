public class ProductSearchDto
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string ScientificName { get; set; }

    public string Barcode { get; set; }

    public decimal SalePrice { get; set; }

    public int Stock { get; set; }

    public bool IsLowStock { get; set; }
}