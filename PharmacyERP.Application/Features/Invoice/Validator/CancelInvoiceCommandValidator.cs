using FluentValidation;

public class CancelInvoiceCommandValidator : AbstractValidator<CancelInvoiceCommand>
{
    public CancelInvoiceCommandValidator()
    {
        RuleFor(x => x.InvoiceId)
            .GreaterThan(0)
            .WithMessage("Invalid invoice id");
    }
}