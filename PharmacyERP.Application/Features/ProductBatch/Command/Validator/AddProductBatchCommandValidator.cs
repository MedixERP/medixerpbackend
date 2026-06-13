using FluentValidation;

public class AddProductBatchCommandValidator
    : AbstractValidator<AddProductBatchCommand>
{
    public AddProductBatchCommandValidator()
    {
       
        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .WithMessage("Invalid product id");

        
        RuleFor(x => x.SupplierId)
            .GreaterThan(0)
            .WithMessage("Invalid supplier id");

       
        RuleFor(x => x.BatchNumber)
            .NotEmpty()
            .WithMessage("Batch number is required")
            .MaximumLength(100);

        
        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0");

        
        RuleFor(x => x.PurchasePrice)
            .GreaterThan(0)
            .WithMessage("Purchase price must be greater than 0");

        
        RuleFor(x => x.ExpiryDate)
            .Must(date => date > DateTime.UtcNow)
            .WithMessage("Expiry date must be in the future");
    }
}