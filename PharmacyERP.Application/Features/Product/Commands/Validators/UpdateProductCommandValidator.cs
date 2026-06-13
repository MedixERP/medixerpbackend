using FluentValidation;

public class UpdateProductCommandValidator
    : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
      
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Invalid product id");

        
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Product name is required")
            .MinimumLength(2)
            .MaximumLength(150);

        
        RuleFor(x => x.PurchasePrice)
            .GreaterThan(0)
            .WithMessage("Purchase price must be greater than 0");

        
        RuleFor(x => x.SalePrice)
            .GreaterThan(x => x.PurchasePrice)
            .WithMessage("Sale price must be greater than purchase price");

        
        RuleFor(x => x.MinStockLevel)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Minimum stock level cannot be negative");
    }
}