using MediatR;
using PharmacyERP.Application.Common.Models;

public class CancelInvoiceCommand : IRequest<Result<string>>
{
    public int InvoiceId { get; set; }
}