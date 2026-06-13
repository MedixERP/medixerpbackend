using MediatR;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;

public class GetAllSalesReturnsQueryHandler
    : IRequestHandler<GetAllSalesReturnsQuery, Result<PaginatedResult<SalesReturnDto>>>
{
    private readonly IUnitOfWork _uow;

    public GetAllSalesReturnsQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<PaginatedResult<SalesReturnDto>>> Handle(
        GetAllSalesReturnsQuery request,
        CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;

        var query = _uow.Repository<SalesReturn>()
            .Query()
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var keyword = request.Keyword.Trim();

            query = query.Where(x =>
                x.Invoice.InvoiceNumber.Contains(keyword) ||
                x.Reason.Contains(keyword));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var data = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(r => new SalesReturnDto
            {
                Id = r.Id,
                InvoiceId = r.InvoiceId,
                InvoiceNumber = r.Invoice.InvoiceNumber,
                ReturnedBy = r.ReturnedByUserId.ToString(),
                Reason = r.Reason,
                TotalAmount = r.TotalAmount,
                CreatedAt = r.CreatedAt,

                Items = r.SalesReturnItems.Select(i => new SalesReturnItemDto
                {
                    ProductName = i.Product.Name,
                    BatchNumber = i.Batch.BatchNumber,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    Total = i.Total
                }).ToList()
            })
            .ToListAsync(cancellationToken);

        var result = new PaginatedResult<SalesReturnDto>(
            data,
            totalCount,
            pageNumber,
            pageSize
        );

        return Result<PaginatedResult<SalesReturnDto>>
            .Success(result, "Sales returns retrieved successfully");
    }
}