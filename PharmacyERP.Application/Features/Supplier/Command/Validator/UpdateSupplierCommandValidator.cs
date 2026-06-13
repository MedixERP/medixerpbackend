using FluentValidation;

public class UpdateSupplierCommandValidator
    : AbstractValidator<UpdateSupplierCommand>
{
    public UpdateSupplierCommandValidator()
    {
        
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Invalid supplier id");

        
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

       
        
        RuleFor(x => x.Phone)
            .NotEmpty()
            .Matches(@"^[0-9+\-\s]{7,20}$");

        
        RuleFor(x => x.Address)
            .NotEmpty()
            .MaximumLength(250);
    }
}