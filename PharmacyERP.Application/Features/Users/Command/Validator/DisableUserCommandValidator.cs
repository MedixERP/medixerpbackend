using FluentValidation;

public class DisableUserCommandValidator : AbstractValidator<DisableUserCommand>
{
    public DisableUserCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}