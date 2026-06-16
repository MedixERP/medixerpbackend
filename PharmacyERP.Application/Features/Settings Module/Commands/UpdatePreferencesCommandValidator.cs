using FluentValidation;

public class UpdatePreferencesCommandValidator
    : AbstractValidator<UpdatePreferencesCommand>
{
    public UpdatePreferencesCommandValidator()
    {
        RuleFor(x => x.Language)
            .NotEmpty().WithMessage("Language is required")
            .Must(x => x == "en" || x == "ar")
            .WithMessage("Language must be 'en' or 'ar'");

        RuleFor(x => x.Theme)
            .NotEmpty().WithMessage("Theme is required")
            .Must(x => x == "light" || x == "dark")
            .WithMessage("Theme must be 'light' or 'dark'");

       
    }
}