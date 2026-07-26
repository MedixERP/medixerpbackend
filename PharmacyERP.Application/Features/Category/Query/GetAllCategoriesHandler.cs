using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;
using PharmacyERP.Application.Features.Category.DTOs;

public class GetAllCategoriesHandler
    : IRequestHandler<GetAllCategoriesQuery,
        Result<PaginatedResult<CategoryDto>>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICacheService _cache;

    public GetAllCategoriesHandler(IUnitOfWork uow, ICacheService cache)
    {
        _uow = uow;
        _cache = cache;
    }

    public async Task<Result<PaginatedResult<CategoryDto>>> Handle(
        GetAllCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;

        var cacheKey =
            $"categories:{pageNumber}:{pageSize}:{request.Keyword}";

        var cachedData =
            await _cache.GetAsync<PaginatedResult<CategoryDto>>(cacheKey, cancellationToken);

        if (cachedData is not null)
        {
            return Result<PaginatedResult<CategoryDto>>
                .Success(cachedData, "Categories retrieved from cache");
        }

        var query = _uow.Repository<Category>()
            .Query()
            .AsNoTracking()
            .Where(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var keyword = request.Keyword.Trim();
            query = query.Where(x =>
                x.Name.Contains(keyword) ||
                x.Description.Contains(keyword));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var data = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            })
            .ToListAsync(cancellationToken);

        var result = new PaginatedResult<CategoryDto>(
            data,
            totalCount,
            pageNumber,
            pageSize
        );

        await _cache.SetAsync(
            cacheKey,
            result,
            TimeSpan.FromMinutes(30),
            cancellationToken);

        return Result<PaginatedResult<CategoryDto>>
            .Success(result, "Categories retrieved successfully");
    }
}