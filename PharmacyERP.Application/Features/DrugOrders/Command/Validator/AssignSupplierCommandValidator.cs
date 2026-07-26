using FluentValidation;

public class AssignSupplierCommandValidator
    : AbstractValidator<AssignSupplierCommand>
{
    public AssignSupplierCommandValidator()
    {
        RuleFor(x => x.OrderId).GreaterThan(0);
        RuleFor(x => x.SupplierName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.SupplierPhone).NotEmpty().MaximumLength(20);
    }
}