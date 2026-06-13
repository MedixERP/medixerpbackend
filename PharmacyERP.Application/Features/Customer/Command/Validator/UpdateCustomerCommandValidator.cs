using FluentValidation;

public class UpdateCustomerCommandValidator
    : AbstractValidator<UpdateCustomerCommand>
{
    public UpdateCustomerCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);

        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Phone)
            .NotEmpty()
            .Matches(@"^01[0-2,5]{1}[0-9]{8}$");

        RuleFor(x => x.Address)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.CreditLimit)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x)
            .Must(x => !x.IsVip || x.CreditLimit >= 1000)
            .WithMessage("VIP customers must have credit limit >= 1000");
    }
}