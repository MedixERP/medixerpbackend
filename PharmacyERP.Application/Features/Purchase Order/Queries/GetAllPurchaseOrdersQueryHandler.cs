using MediatR;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;

public class GetAllPurchaseOrdersQueryHandler
    : IRequestHandler<GetAllPurchaseOrdersQuery, Result<PaginatedResult<PurchaseOrderDto>>>
{
    private readonly IUnitOfWork _uow;

    public GetAllPurchaseOrdersQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<PaginatedResult<PurchaseOrderDto>>> Handle(
        GetAllPurchaseOrdersQuery request,
        CancellationToken cancellationToken)
    {
        
        var pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;

       
        var query = _uow.Repository<PurchaseOrder>()
            .Query()
            .AsNoTracking()
            .Include(x => x.Supplier)
            .Include(x => x.PurchaseOrderItems)
            .Where(x => !x.IsDeleted);

        
        if (request.SupplierId.HasValue)
        {
            query = query.Where(x => x.SupplierId == request.SupplierId.Value);
        }

        
        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            query = query.Where(x => x.Status.ToString() == request.Status);
        }

        
        var totalCount = await query.CountAsync(cancellationToken);

        
        var orders = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(o => new PurchaseOrderDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                SupplierId = o.SupplierId,
                SupplierName = o.Supplier.Name,
                Status = o.Status.ToString(),
                CreatedAt = o.CreatedAt,

                ItemsCount = o.PurchaseOrderItems.Count,

                TotalAmount = o.PurchaseOrderItems
                    .Sum(i => i.Quantity * i.UnitPrice)
            })
            .ToListAsync(cancellationToken);

       
        var paginatedResult = new PaginatedResult<PurchaseOrderDto>(
            orders,
            totalCount,
            pageNumber,
            pageSize
        );

        return Result<PaginatedResult<PurchaseOrderDto>>
            .Success(paginatedResult, "Purchase orders retrieved successfully");
    }
}