using FluentValidation;

public class CreatePurchaseOrderCommandValidator
    : AbstractValidator<CreatePurchaseOrderCommand>
{
    public CreatePurchaseOrderCommandValidator()
    {
       
        RuleFor(x => x.SupplierId)
            .GreaterThan(0)
            .WithMessage("Invalid supplier id");

        
        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("Purchase order must contain at least one item");

        
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId)
                .GreaterThan(0)
                .WithMessage("Invalid product id");

            item.RuleFor(i => i.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than 0");

            item.RuleFor(i => i.UnitPrice)
                .GreaterThan(0)
                .WithMessage("Unit price must be greater than 0");
        });

        
        RuleFor(x => x.Items)
            .Must(items => items.Select(i => i.ProductId).Distinct().Count() == items.Count)
            .WithMessage("Duplicate products are not allowed in the same purchase order");
    }
}