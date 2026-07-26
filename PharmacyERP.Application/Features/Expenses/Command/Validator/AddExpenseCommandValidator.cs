using FluentValidation;

public class AddExpenseCommandValidator : AbstractValidator<AddExpenseCommand>
{
    public AddExpenseCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than 0");
    }
}