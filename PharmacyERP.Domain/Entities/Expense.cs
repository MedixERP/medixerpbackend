namespace PharmacyERP.Domain.Entities;

public class Expense : BaseEntity
{
    public string Title { get; set; }
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaidAt { get; set; }
    public int CreatedByUserId { get; set; }
    public ApplicationUser CreatedByUser { get; set; }
}