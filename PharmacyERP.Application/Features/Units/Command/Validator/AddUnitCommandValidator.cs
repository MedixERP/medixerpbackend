using FluentValidation;

public class AddUnitCommandValidator : AbstractValidator<AddUnitCommand>
{
    public AddUnitCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Symbol)
            .NotEmpty()
            .MaximumLength(10);
    }
}