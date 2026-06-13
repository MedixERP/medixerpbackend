using FluentValidation;

public class AddSupplierCommandValidator
    : AbstractValidator<AddSupplierCommand>
{
    public AddSupplierCommandValidator()
    {
        
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Supplier name is required")
            .MinimumLength(2)
            .MaximumLength(100);

        
        RuleFor(x => x.Phone)
            .NotEmpty()
            .WithMessage("Phone is required")
            .Matches(@"^[0-9+\-\s]{7,20}$")
            .WithMessage("Invalid phone format");

        
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Invalid email format")
            .MaximumLength(150);

       
        RuleFor(x => x.Address)
            .NotEmpty()
            .WithMessage("Address is required")
            .MaximumLength(250);
    }
}