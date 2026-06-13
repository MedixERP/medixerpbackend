using PharmacyERP.Domain.Enums;

namespace PharmacyERP.Domain.Entities;

public class Invoice : BaseEntity
{
    public string InvoiceNumber { get; set; }

    public int CustomerId { get; set; }

    public int CreatedByUserId { get; set; }

    public decimal TotalAmount { get; set; }

    public decimal Discount { get; set; }

    public decimal FinalAmount { get; set; }

    public PaymentStatus PaymentStatus { get; set; }

    public bool IsCancelled { get; set; }

    public Customer Customer { get; set; }

    public ApplicationUser CreatedByUser { get; set; }

    public ICollection<InvoiceItem> InvoiceItems { get; set; }

    public ICollection<Payment> Payments { get; set; }

    public ICollection<SalesReturn> SalesReturns { get; set; }
}