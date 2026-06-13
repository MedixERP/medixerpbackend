
namespace PharmacyERP.Domain.Entities;

public class SalesReturn : BaseEntity
{
    public int InvoiceId { get; set; }

    public int ReturnedByUserId { get; set; }

    public string Reason { get; set; }

    public decimal TotalAmount { get; set; }

    public Invoice Invoice { get; set; }

    public ApplicationUser ReturnedByUser { get; set; }

    public ICollection<SalesReturnItem> SalesReturnItems { get; set; }
}