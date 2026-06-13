using FluentValidation;

public class ReceivePurchaseOrderCommandValidator
    : AbstractValidator<ReceivePurchaseOrderCommand>
{
    public ReceivePurchaseOrderCommandValidator()
    {
        RuleFor(x => x.PurchaseOrderId)
            .GreaterThan(0)
            .WithMessage("Invalid purchase order id");
    }
}