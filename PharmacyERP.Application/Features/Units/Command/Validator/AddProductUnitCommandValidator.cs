using FluentValidation;

public class AddProductUnitCommandValidator
    : AbstractValidator<AddProductUnitCommand>
{
    public AddProductUnitCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .WithMessage("Product is required");

        RuleFor(x => x.UnitId)
            .GreaterThan(0)
            .WithMessage("Unit is required");

        RuleFor(x => x.ConversionFactor)
            .GreaterThan(0)
            .WithMessage("Conversion factor must be greater than 0");
    }
}