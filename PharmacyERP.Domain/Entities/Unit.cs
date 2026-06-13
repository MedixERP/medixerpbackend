
namespace PharmacyERP.Domain.Entities;

public class Unit : BaseEntity
{
    public string Name { get; set; }

    public string Symbol { get; set; }

    public ICollection<ProductUnit> ProductUnits { get; set; }
}