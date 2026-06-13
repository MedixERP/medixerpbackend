
namespace PharmacyERP.Domain.Entities;

public class Customer : BaseEntity
{
    public string FullName { get; set; }

    public string Phone { get; set; }

    public string Address { get; set; }

    public bool IsVip { get; set; }

    public decimal CreditLimit { get; set; }

    public ICollection<Invoice> Invoices { get; set; }
}