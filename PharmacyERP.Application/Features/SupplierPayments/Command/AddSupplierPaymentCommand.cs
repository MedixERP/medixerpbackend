using MediatR;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Application.Features.SupplierPayments.Queries;

namespace PharmacyERP.Application.Features.SupplierPayments.Command
{
   
    public class AddSupplierPaymentCommand : IRequest<Result<SupplierDebtDto>>
    {
        public int SupplierId { get; set; }
        public int PurchaseOrderId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
    }
}
