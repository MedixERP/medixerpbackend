using MediatR;
using PharmacyERP.Application.Common.Models;

public class GetInvoiceByIdQuery
    : IRequest<Result<InvoiceDto>>
{
    public int Id { get; set; }
}