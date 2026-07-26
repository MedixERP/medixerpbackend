using FluentValidation;
using PharmacyERP.Application.Features.SupplierPayments.Command;

public class AddSupplierPaymentCommandValidator
    : AbstractValidator<AddSupplierPaymentCommand>
{
    public AddSupplierPaymentCommandValidator()
    {
        RuleFor(x => x.SupplierId).GreaterThan(0);
        RuleFor(x => x.PurchaseOrderId).GreaterThan(0);
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.PaymentMethod).NotEmpty();
    }
}