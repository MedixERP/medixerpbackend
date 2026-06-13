using FluentValidation;

public class ScanBarcodeToInvoiceCommandValidator
    : AbstractValidator<ScanBarcodeToInvoiceCommand>
{
    public ScanBarcodeToInvoiceCommandValidator()
    {
       
        RuleFor(x => x.InvoiceId)
            .GreaterThan(0)
            .WithMessage("Invalid invoice id");

      
        RuleFor(x => x.Barcode)
            .NotEmpty()
            .WithMessage("Barcode is required")
            .MaximumLength(100);

        
        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0")
            .LessThanOrEqualTo(100)
            .WithMessage("Quantity cannot exceed 100 per scan");
    }
}