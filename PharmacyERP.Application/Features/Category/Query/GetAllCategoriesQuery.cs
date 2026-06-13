using MediatR;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Application.Features.Category.DTOs;

public class GetAllCategoriesQuery
    : PaginationRequest,
      IRequest<Result<PaginatedResult<CategoryDto>>>
{
    public string? Keyword { get; set; }
}