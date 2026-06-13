
namespace PharmacyERP.Domain.Entities;

public class SystemSetting : BaseEntity
{
    public string Key { get; set; }

    public string Value { get; set; }

    public string Description { get; set; }
}