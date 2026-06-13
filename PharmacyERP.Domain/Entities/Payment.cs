
namespace PharmacyERP.Domain.Entities;

public class Payment : BaseEntity
{
    public int InvoiceId { get; set; }

    public decimal Amount { get; set; }

    public string PaymentMethod { get; set; }

    public DateTime PaidAt { get; set; }

    public Invoice Invoice { get; set; }
}