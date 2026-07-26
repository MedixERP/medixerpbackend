using FluentValidation;

public class CreateDrugOrderCommandValidator
    : AbstractValidator<CreateDrugOrderCommand>
{
    public CreateDrugOrderCommandValidator()
    {
        RuleFor(x => x.PharmacyCompanyId)
            .GreaterThan(0)
            .WithMessage("Company is required");

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("Order must have at least one item");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId)
                .GreaterThan(0)
                .WithMessage("Product is required");

            item.RuleFor(i => i.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than 0");

            item.RuleFor(i => i.UnitPrice)
                .GreaterThan(0)
                .WithMessage("Unit price must be greater than 0");
        });
    }
}