using FluentValidation;

public class CreateSalesReturnCommandValidator
    : AbstractValidator<CreateSalesReturnCommand>
{
    public CreateSalesReturnCommandValidator()
    {
        
        RuleFor(x => x.InvoiceId)
            .GreaterThan(0)
            .WithMessage("Invalid invoice id");

        
        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Reason is required")
            .MaximumLength(250);

       
        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("Return must contain at least one item");

       
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId)
                .GreaterThan(0)
                .WithMessage("Invalid product id");

            item.RuleFor(i => i.BatchId)
                .GreaterThan(0)
                .WithMessage("Invalid batch id");

            item.RuleFor(i => i.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than 0");
        });

       
        RuleFor(x => x.Items)
            .Must(items =>
                items.Select(i => (i.ProductId, i.BatchId)).Distinct().Count()
                == items.Count)
            .WithMessage("Duplicate product/batch combinations are not allowed");
    }
}