using PharmacyERP.Domain.Entities;

public class PharmacyCompany : BaseEntity
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public bool IsActive { get; set; } = true;

    public int UserId { get; set; }
    public ApplicationUser User { get; set; }

    public ICollection<DrugOrder> DrugOrders { get; set; }
}