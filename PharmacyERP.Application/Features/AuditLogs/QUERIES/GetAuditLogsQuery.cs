using MediatR;
using PharmacyERP.Application.Common.Models;

public class GetAuditLogsQuery
    : PaginationRequest,
      IRequest<Result<PaginatedResult<AuditLogDto>>>
{
    public string? EntityName { get; set; }
    public string? Action { get; set; }
    public int? UserId { get; set; }
}