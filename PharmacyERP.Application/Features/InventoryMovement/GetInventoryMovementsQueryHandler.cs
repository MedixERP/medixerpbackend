using MediatR;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;
using PharmacyERP.Domain.Enums;

public class GetInventoryMovementsQueryHandler
    : IRequestHandler<GetInventoryMovementsQuery, Result<PaginatedResult<InventoryMovementDto>>>
{
    private readonly IUnitOfWork _uow;

    public GetInventoryMovementsQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<PaginatedResult<InventoryMovementDto>>> Handle(
        GetInventoryMovementsQuery request,
        CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;

        var query = _uow.Repository<InventoryMovement>()
            .Query()
            .AsNoTracking()
            .Include(x => x.Product)
            .Include(x => x.User)
            .Where(x => !x.IsDeleted);

        if (request.ProductId.HasValue)
            query = query.Where(x => x.ProductId == request.ProductId.Value);

        if (!string.IsNullOrWhiteSpace(request.Type) &&
            Enum.TryParse<InventoryMovementType>(request.Type, true, out var parsedType))
        {
            query = query.Where(x => x.Type == parsedType);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var data = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new InventoryMovementDto
            {
                Id = x.Id,
                ProductName = x.Product != null ? x.Product.Name : "",
                Type = x.Type.ToString(),
                Quantity = x.Quantity,
                BeforeQuantity = x.BeforeQuantity,
                AfterQuantity = x.AfterQuantity,
                Reason = x.Reason,
                ReferenceType = x.ReferenceType,
                ReferenceId = x.ReferenceId,
                CreatedBy = x.User != null ? x.User.FullName : "",
                CreatedAt = x.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return Result<PaginatedResult<InventoryMovementDto>>.Success(
            new PaginatedResult<InventoryMovementDto>(data, totalCount, pageNumber, pageSize),
            "Inventory movements retrieved successfully"
        );
    }
}