
namespace PharmacyERP.Domain.Entities;

public class AuditLog : BaseEntity
{
    public int UserId { get; set; }

    public string Action { get; set; }

    public string EntityName { get; set; }

    public string EntityId { get; set; }

    public string OldValues { get; set; }

    public string NewValues { get; set; }

    public ApplicationUser User { get; set; }
}