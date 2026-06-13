using MediatR;
using PharmacyERP.Application.Common.Models;

public class RemoveInvoiceItemCommand : IRequest<Result<int>>
{
    public int InvoiceId { get; set; }
    public int InvoiceItemId { get; set; }
}