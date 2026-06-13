using MediatR;
using PharmacyERP.Application.Common.Models;

public class CreateInvoiceCommand : IRequest<Result<int>>
{
    public int CustomerId { get; set; }
    public List<CreateInvoiceItemDto> Items { get; set; }
}