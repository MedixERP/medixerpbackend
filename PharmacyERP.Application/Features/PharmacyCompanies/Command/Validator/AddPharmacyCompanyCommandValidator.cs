using FluentValidation;

public class AddPharmacyCompanyCommandValidator
    : AbstractValidator<AddPharmacyCompanyCommand>
{
    public AddPharmacyCompanyCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Phone)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.Address)
            .NotEmpty()
            .MaximumLength(300);
    }
}