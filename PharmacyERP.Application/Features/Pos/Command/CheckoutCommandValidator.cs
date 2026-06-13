using FluentValidation;

public class CheckoutCommandValidator : AbstractValidator<CheckoutCommand>
{
    public CheckoutCommandValidator()
    {
        RuleFor(x => x.PaidAmount)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Paid amount cannot be negative");

        RuleFor(x => x.CustomerId)
            .GreaterThan(0)
            .When(x => x.CustomerId.HasValue)
            .WithMessage("Invalid customer id");
    }
}