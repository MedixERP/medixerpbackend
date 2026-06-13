
namespace PharmacyERP.Domain.Entities;

public class ProductUnit : BaseEntity
{
    public int ProductId { get; set; }

    public int UnitId { get; set; }


    public int ConversionFactor { get; set; }

    public bool IsBaseUnit { get; set; }


    public Product Product { get; set; }

    public Unit Unit { get; set; }
}