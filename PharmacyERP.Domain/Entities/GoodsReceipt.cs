

namespace PharmacyERP.Domain.Entities;

public class GoodsReceipt : BaseEntity
{
    public int PurchaseOrderId { get; set; }

    public int ReceivedByUserId { get; set; }

    public DateTime ReceivedAt { get; set; }

    public PurchaseOrder PurchaseOrder { get; set; }

    public ApplicationUser ReceivedByUser { get; set; }
}