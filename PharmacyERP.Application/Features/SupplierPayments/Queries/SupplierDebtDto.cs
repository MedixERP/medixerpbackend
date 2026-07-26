

namespace PharmacyERP.Application.Features.SupplierPayments.Queries
{
    public class SupplierDebtDto
    {
        public int SupplierId { get; set; }
        public int PurchaseOrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal Remaining { get; set; }
    }
}
