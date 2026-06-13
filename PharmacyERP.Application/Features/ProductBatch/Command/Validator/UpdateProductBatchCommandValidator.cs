using FluentValidation;

public class UpdateProductBatchCommandValidator
    : AbstractValidator<UpdateProductBatchCommand>
{
    public UpdateProductBatchCommandValidator()
    {
        
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Invalid batch id");

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Quantity cannot be negative");

        
        RuleFor(x => x.PurchasePrice)
            .GreaterThan(0)
            .WithMessage("Purchase price must be greater than 0");

        
        RuleFor(x => x.ExpiryDate)
            .Must(date => date > DateTime.UtcNow)
            .WithMessage("Expiry date must be in the future");
    }
}