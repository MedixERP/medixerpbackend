public class InventoryMovementDto
{
    public int Id { get; set; }


public string ProductName { get; set; }

    public string Type { get; set; }

    public int Quantity { get; set; }

    public int BeforeQuantity { get; set; }

    public int AfterQuantity { get; set; }

    public string Reason { get; set; }

    public string ReferenceType { get; set; }

    public int? ReferenceId { get; set; }

    public string CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }


}
