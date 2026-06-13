namespace PharmacyERP.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; }
    public string ScientificName { get; set; }


    public string Barcode { get; set; }

    public int CategoryId { get; set; }

    public decimal PurchasePrice { get; set; }
    public decimal SalePrice { get; set; }

 
    public int MinStockLevel { get; set; }
    public bool IsActive { get; set; }


    public byte[]? BarcodeImage { get; set; }
    public byte[]? QrCodeImage { get; set; }


    public byte[] RowVersion { get; set; }

  
    public Category Category { get; set; }

    public ICollection<ProductBatch> ProductBatches { get; set; } = new List<ProductBatch>();
    public ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
    public ICollection<InventoryMovement> InventoryMovements { get; set; } = new List<InventoryMovement>();
    public ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; } = new List<PurchaseOrderItem>();
    public ICollection<ProductUnit> ProductUnits { get; set; } = new List<ProductUnit>();
    public ICollection<SalesReturnItem> SalesReturnItems { get; set; } = new List<SalesReturnItem>();
}