using MediatR;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;

public class GetAuditLogsQueryHandler
    : IRequestHandler<GetAuditLogsQuery,
        Result<PaginatedResult<AuditLogDto>>>
{
    private readonly IUnitOfWork _uow;

    public GetAuditLogsQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<PaginatedResult<AuditLogDto>>> Handle(
        GetAuditLogsQuery request,
        CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;

        var query = _uow.Repository<AuditLog>()
            .Query()
            .AsNoTracking()
            .Include(x => x.User)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.EntityName))
            query = query.Where(x => x.EntityName == request.EntityName);

        if (!string.IsNullOrWhiteSpace(request.Action))
            query = query.Where(x => x.Action == request.Action);

        if (request.UserId.HasValue)
            query = query.Where(x => x.UserId == request.UserId.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var data = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new AuditLogDto
            {
                Id = x.Id,
                UserName = x.User != null ? x.User.FullName : "",
                Action = x.Action,
                EntityName = x.EntityName,
                EntityId = x.EntityId,
                OldValues = x.OldValues,
                NewValues = x.NewValues,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return Result<PaginatedResult<AuditLogDto>>.Success(
            new PaginatedResult<AuditLogDto>(
                data, totalCount, pageNumber, pageSize),
            "Audit logs retrieved successfully");
    }
}