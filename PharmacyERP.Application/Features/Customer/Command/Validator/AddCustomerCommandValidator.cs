using FluentValidation;

public class AddCustomerCommandValidator
    : AbstractValidator<AddCustomerCommand>
{
    public AddCustomerCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage("Customer name is required")
            .MaximumLength(100)
            .WithMessage("Name must not exceed 100 characters");

        RuleFor(x => x.Phone)
            .NotEmpty()
            .WithMessage("Phone is required")
            .Matches(@"^01[0-2,5]{1}[0-9]{8}$")
            .WithMessage("Invalid Egyptian phone number format");

        RuleFor(x => x.Address)
            .NotEmpty()
            .WithMessage("Address is required")
            .MaximumLength(200)
            .WithMessage("Address is too long");

        RuleFor(x => x.CreditLimit)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Credit limit cannot be negative");

        RuleFor(x => x)
            .Must(x => !x.IsVip || x.CreditLimit >= 1000)
            .WithMessage("VIP customers must have credit limit >= 1000");
    }
}