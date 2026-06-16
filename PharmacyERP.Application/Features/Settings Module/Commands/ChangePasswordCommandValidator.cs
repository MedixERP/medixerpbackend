using FluentValidation;

public class ChangePasswordCommandValidator
    : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("Current password is required")
            .MinimumLength(6).WithMessage("Current password must be at least 6 characters")
            .MaximumLength(100);

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters")
            .MaximumLength(100)
            .Must(ContainUpperCase).WithMessage("Must contain uppercase letter")
            .Must(ContainLowerCase).WithMessage("Must contain lowercase letter")
            .Must(ContainDigit).WithMessage("Must contain number")
            .Must(ContainSpecialChar).WithMessage("Must contain special character");

        RuleFor(x => x)
            .Must(x => x.CurrentPassword != x.NewPassword)
            .WithMessage("New password must be different from current password");
    }

    private bool ContainUpperCase(string password)
        => password.Any(char.IsUpper);

    private bool ContainLowerCase(string password)
        => password.Any(char.IsLower);

    private bool ContainDigit(string password)
        => password.Any(char.IsDigit);

    private bool ContainSpecialChar(string password)
        => password.Any(ch => !char.IsLetterOrDigit(ch));
}