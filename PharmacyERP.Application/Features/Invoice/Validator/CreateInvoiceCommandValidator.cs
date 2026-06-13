using FluentValidation;

public class CreateInvoiceCommandValidator : AbstractValidator<CreateInvoiceCommand>
{
    public CreateInvoiceCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .GreaterThan(0)
            .WithMessage("Customer is required");

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("Invoice must have at least one item");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId)
                .GreaterThan(0)
                .WithMessage("Invalid product");

            item.RuleFor(i => i.BatchId)
                .GreaterThan(0)
                .WithMessage("Invalid batch");

            item.RuleFor(i => i.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than 0");
        });

        RuleFor(x => x.Items)
            .Must(items => items.GroupBy(i => i.ProductId).All(g => g.Count() == 1))
            .WithMessage("Duplicate products in invoice are not allowed");
    }
}