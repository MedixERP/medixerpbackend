using MediatR;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;
using PharmacyERP.Domain.Enums;

public class GetAllDrugOrdersHandler
    : IRequestHandler<GetAllDrugOrdersQuery,
        Result<PaginatedResult<DrugOrderDto>>>
{
    private readonly IUnitOfWork _uow;

    public GetAllDrugOrdersHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<PaginatedResult<DrugOrderDto>>> Handle(
        GetAllDrugOrdersQuery request,
        CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;

        var query = _uow.Repository<DrugOrder>()
            .Query()
            .AsNoTracking()
            .Include(x => x.PharmacyCompany)
            .Include(x => x.CreatedByUser)
            .Include(x => x.Items)
                .ThenInclude(i => i.Product)
            .Where(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.Status) &&
            Enum.TryParse<DrugOrderStatus>(
                request.Status, true, out var status))
            query = query.Where(x => x.Status == status);

        if (request.CompanyId.HasValue)
            query = query.Where(
                x => x.PharmacyCompanyId == request.CompanyId.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var data = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new DrugOrderDto
            {
                Id = x.Id,
                OrderNumber = x.OrderNumber,
                CompanyName = x.PharmacyCompany.Name,
                CreatedBy = x.CreatedByUser.FullName,
                Status = x.Status.ToString(),
                RejectionReason = x.RejectionReason,
                SupplierName = x.SupplierName,
                SupplierPhone = x.SupplierPhone,
                TotalAmount = x.TotalAmount,
                ReceivedAt = x.ReceivedAt,
                CreatedAt = x.CreatedAt,
                Items = x.Items.Select(i => new DrugOrderItemDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product.Name,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    Total = i.Total
                }).ToList()
            })
            .ToListAsync(cancellationToken);

        return Result<PaginatedResult<DrugOrderDto>>.Success(
            new PaginatedResult<DrugOrderDto>(
                data, totalCount, pageNumber, pageSize),
            "Orders retrieved successfully");
    }
}