using FluentValidation;

public class ChangeUserRoleCommandValidator : AbstractValidator<ChangeUserRoleCommand>
{
    public ChangeUserRoleCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);

        RuleFor(x => x.NewRole)
            .NotEmpty()
            .WithMessage("Role is required")
            .Must(r => new[] { "Admin", "Pharmacist", "Cashier", "Customer" }
                .Contains(r))
            .WithMessage("Invalid role. Allowed: Admin, Pharmacist, Cashier, Customer");
    }
}