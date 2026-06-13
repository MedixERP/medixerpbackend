using MediatR;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;

public class GetAllInvoicesQueryHandler
    : IRequestHandler<GetAllInvoicesQuery, Result<PaginatedResult<InvoiceDto>>>
{
    private readonly IUnitOfWork _uow;

    public GetAllInvoicesQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<PaginatedResult<InvoiceDto>>> Handle(
        GetAllInvoicesQuery request,
        CancellationToken cancellationToken)
    {
        
        var pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;

       
        var query = _uow.Repository<Invoice>()
            .Query()
            .AsNoTracking()
            .Include(x => x.Customer)
            .Include(x => x.InvoiceItems)
                .ThenInclude(i => i.Product)
            .Include(x => x.InvoiceItems)
                .ThenInclude(i => i.Batch)
            .Where(x => !x.IsDeleted);

       
        if (request.CustomerId.HasValue)
        {
            query = query.Where(x => x.CustomerId == request.CustomerId.Value);
        }

       
        if (request.IsCancelled.HasValue)
        {
            query = query.Where(x => x.IsCancelled == request.IsCancelled.Value);
        }

     
        var totalCount = await query.CountAsync(cancellationToken);

       
        var invoices = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(invoice => new InvoiceDto
            {
                Id = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                CustomerName = invoice.Customer.FullName,
                TotalAmount = invoice.TotalAmount,
                FinalAmount = invoice.FinalAmount,
                IsCancelled = invoice.IsCancelled,

                Items = invoice.InvoiceItems.Select(i => new InvoiceItemDto
                {
                    ProductId = i.ProductId,
                    BatchId = i.BatchId,
                    ProductName = i.Product.Name,
                    BatchNumber = i.Batch.BatchNumber,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    Total = i.Total
                }).ToList()
            })
            .ToListAsync(cancellationToken);

       
        var paginatedResult = new PaginatedResult<InvoiceDto>(
            invoices,
            totalCount,
            pageNumber,
            pageSize
        );

        return Result<PaginatedResult<InvoiceDto>>
            .Success(paginatedResult, "Invoices retrieved successfully");
    }
}