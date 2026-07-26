using MediatR;
using PharmacyERP.Application.Common.Models;

public class AddInvoicePaymentCommand : IRequest<Result<CustomerDebtDto>>
{
    public int InvoiceId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; }
}