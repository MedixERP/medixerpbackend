using FluentValidation;

public class AddProductCommandValidator : AbstractValidator<AddProductCommand>
{
    public AddProductCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.PurchasePrice).GreaterThan(0);
        RuleFor(x => x.SalePrice)
            .GreaterThan(x => x.PurchasePrice)
            .WithMessage("Sale price must be greater than purchase price");
    }
}