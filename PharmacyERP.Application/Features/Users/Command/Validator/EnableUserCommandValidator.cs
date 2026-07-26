using FluentValidation;

public class EnableUserCommandValidator : AbstractValidator<EnableUserCommand>
{
    public EnableUserCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}