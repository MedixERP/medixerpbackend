using FluentValidation;

public class RemoveInvoiceItemCommandValidator
    : AbstractValidator<RemoveInvoiceItemCommand>
{
    public RemoveInvoiceItemCommandValidator()
    {
        
        RuleFor(x => x.InvoiceId)
            .GreaterThan(0)
            .WithMessage("Invalid invoice id");

       
        RuleFor(x => x.InvoiceItemId)
            .GreaterThan(0)
            .WithMessage("Invalid invoice item id");
    }
}