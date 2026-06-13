using MediatR;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;

public class GetInvoiceByIdQueryHandler
    : IRequestHandler<GetInvoiceByIdQuery, Result<InvoiceDto>>
{
    private readonly IUnitOfWork _uow;

    public GetInvoiceByIdQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<InvoiceDto>> Handle(
        GetInvoiceByIdQuery request,
        CancellationToken cancellationToken)
    {
        var invoice = await _uow.Repository<Invoice>()
            .Query()
            .Include(x => x.Customer)
            .Include(x => x.InvoiceItems)
                .ThenInclude(i => i.Product)
            .Include(x => x.InvoiceItems)
                .ThenInclude(i => i.Batch)
            .FirstOrDefaultAsync(
                x => x.Id == request.Id,
                cancellationToken);

        if (invoice == null)
        {
            return Result<InvoiceDto>.Failure(
                "Invoice not found",
                404);
        }

        var dto = new InvoiceDto
        {
            Id = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            CustomerName = invoice.Customer.FullName,
            TotalAmount = invoice.TotalAmount,
            FinalAmount = invoice.FinalAmount,
            IsCancelled = invoice.IsCancelled,

            Items = invoice.InvoiceItems.Select(i => new InvoiceItemDto
            {
                ProductName = i.Product.Name,
                BatchNumber = i.Batch.BatchNumber,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                Total = i.Total
            }).ToList()
        };

        return Result<InvoiceDto>.Success(
            dto,
            "Invoice retrieved successfully");
    }
}