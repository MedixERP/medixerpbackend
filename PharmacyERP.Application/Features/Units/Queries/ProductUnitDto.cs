public class ProductUnitDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public int UnitId { get; set; }
    public string UnitName { get; set; }
    public string UnitSymbol { get; set; }
    public int ConversionFactor { get; set; }
    public bool IsBaseUnit { get; set; }
}

public class ConvertUnitResultDto
{
    public string FromUnit { get; set; }
    public string ToUnit { get; set; }
    public decimal OriginalQuantity { get; set; }
    public decimal ConvertedQuantity { get; set; }
}