using MediatR;
using PharmacyERP.Application.Common.Models;

public class GetAllUsersQuery
    : PaginationRequest,
      IRequest<Result<PaginatedResult<UserDto>>>
{
    public string? Keyword { get; set; }
    public string? Role { get; set; }
    public bool? IsActive { get; set; }
}