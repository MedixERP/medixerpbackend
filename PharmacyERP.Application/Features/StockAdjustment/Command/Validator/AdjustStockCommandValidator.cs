using FluentValidation;

public class AdjustStockCommandValidator : AbstractValidator<AdjustStockCommand>
{
    public AdjustStockCommandValidator()
    {
        RuleFor(x => x.BatchId)
            .GreaterThan(0)
            .WithMessage("Invalid batch id");

        RuleFor(x => x.NewQuantity)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Quantity cannot be negative");

        RuleFor(x => x.ReasonType)
            .IsInEnum()
            .WithMessage("Invalid adjustment reason. You must choose (Broken, Damaged, or Lost).");
    }
}