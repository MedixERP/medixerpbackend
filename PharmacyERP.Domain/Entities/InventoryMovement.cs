using PharmacyERP.Domain.Enums;

namespace PharmacyERP.Domain.Entities;

public class InventoryMovement : BaseEntity
{
    public int ProductId { get; set; }


public int? BatchId { get; set; }

    public InventoryMovementType Type { get; set; }

    public int Quantity { get; set; }

    public int BeforeQuantity { get; set; }

    public int AfterQuantity { get; set; }

    public string? Reason { get; set; }

    public string? ReferenceType { get; set; }

    public int? ReferenceId { get; set; }

    public int UserId { get; set; }

    public Product Product { get; set; }

    public ProductBatch? Batch { get; set; }

    public ApplicationUser User { get; set; }


}
