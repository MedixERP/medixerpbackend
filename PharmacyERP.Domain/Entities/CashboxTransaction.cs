using PharmacyERP.Domain.Enums;

namespace PharmacyERP.Domain.Entities;

public class CashboxTransaction : BaseEntity
{
    public CashboxTransactionType Type { get; set; }
    public CashboxSource Source { get; set; }
    public decimal Amount { get; set; }
    public string? ReferenceType { get; set; }
    public int? ReferenceId { get; set; }
    public string? Description { get; set; }
    public int CreatedByUserId { get; set; }
    public ApplicationUser CreatedByUser { get; set; }
}