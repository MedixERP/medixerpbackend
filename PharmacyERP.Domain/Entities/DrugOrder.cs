using PharmacyERP.Domain.Enums;

namespace PharmacyERP.Domain.Entities;

public class DrugOrder : BaseEntity
{
    public string OrderNumber { get; set; }
    public int PharmacyCompanyId { get; set; }
    public int CreatedByUserId { get; set; }
    public DrugOrderStatus Status { get; set; } = DrugOrderStatus.Pending;
    public string? RejectionReason { get; set; }
    public string? SupplierName { get; set; }
    public string? SupplierPhone { get; set; }
    public DateTime? ReceivedAt { get; set; }
    public decimal TotalAmount { get; set; }
    public PharmacyCompany PharmacyCompany { get; set; }
    public ApplicationUser CreatedByUser { get; set; }
    public ICollection<DrugOrderItem> Items { get; set; }
}