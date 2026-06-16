using FluentValidation;

public class UpdateProfileCommandValidator
    : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("FullName is required")
            .MinimumLength(3).WithMessage("FullName must be at least 3 characters")
            .MaximumLength(100).WithMessage("FullName is too long");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Phone is required")
            .Matches(@"^01[0-2,5]{1}[0-9]{8}$")
            .WithMessage("Invalid Egyptian phone number");
    }
}