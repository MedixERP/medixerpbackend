using FluentValidation;

public class AddInvoicePaymentCommandValidator
    : AbstractValidator<AddInvoicePaymentCommand>
{
    public AddInvoicePaymentCommandValidator()
    {
        RuleFor(x => x.InvoiceId).GreaterThan(0);
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.PaymentMethod).NotEmpty();
    }
}