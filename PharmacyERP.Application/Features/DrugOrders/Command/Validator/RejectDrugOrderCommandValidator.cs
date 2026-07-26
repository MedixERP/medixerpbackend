using FluentValidation;

public class RejectDrugOrderCommandValidator
    : AbstractValidator<RejectDrugOrderCommand>
{
    public RejectDrugOrderCommandValidator()
    {
        RuleFor(x => x.OrderId).GreaterThan(0);
    }
}