using MediatR;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;

public class GetAllSuppliersQueryHandler
    : IRequestHandler<GetAllSuppliersQuery, Result<PaginatedResult<SupplierDto>>>
{
    private readonly IUnitOfWork _uow;

    public GetAllSuppliersQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<PaginatedResult<SupplierDto>>> Handle(
        GetAllSuppliersQuery request,
        CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;

        var query = _uow.Repository<Supplier>()
            .Query()
            .AsNoTracking()
            .Where(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var keyword = request.Keyword.Trim();

            query = query.Where(x =>
                x.Name.Contains(keyword) ||
                x.Phone.Contains(keyword) ||
                x.Address.Contains(keyword));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var data = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new SupplierDto
            {
                Id = x.Id,
                Name = x.Name,
                Phone = x.Phone,
                Address = x.Address
            })
            .ToListAsync(cancellationToken);

        var result = new PaginatedResult<SupplierDto>(
            data,
            totalCount,
            pageNumber,
            pageSize
        );

        return Result<PaginatedResult<SupplierDto>>
            .Success(result, "Suppliers retrieved successfully");
    }
}