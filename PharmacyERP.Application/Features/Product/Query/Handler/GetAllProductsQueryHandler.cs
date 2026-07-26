using Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;

public class GetAllProductsQueryHandler
    : IRequestHandler<GetAllProductsQuery, Result<PaginatedResult<ProductDto>>>
{
    private readonly IUnitOfWork _uow;
    private readonly IHttpContextAccessor _http;
    private readonly ICacheService _cache;

    public GetAllProductsQueryHandler(
        IUnitOfWork uow,
        IHttpContextAccessor http,
        ICacheService cache)
    {
        _uow = uow;
        _http = http;
        _cache = cache;
    }

    public async Task<Result<PaginatedResult<ProductDto>>> Handle(
        GetAllProductsQuery request,
        CancellationToken cancellationToken)
    {
        var user = _http.HttpContext?.User;

        if (user == null || !user.Identity!.IsAuthenticated)
            return Result<PaginatedResult<ProductDto>>
                .Failure("Unauthorized", 401);

        if (!user.IsInRole("Admin") &&
            !user.IsInRole("Pharmacist") &&
            !user.IsInRole("Cashier"))
        {
            return Result<PaginatedResult<ProductDto>>
                .Failure("Forbidden", 403);
        }

        var pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;

        var cacheKey =
            $"products:{pageNumber}:{pageSize}:{request.Keyword}:{request.CategoryId}:{request.IsLowStock}:{request.SortBy}:{request.SortDirection}";

        var cachedData =
            await _cache.GetAsync<PaginatedResult<ProductDto>>(cacheKey, cancellationToken);

        if (cachedData is not null)
        {
            return Result<PaginatedResult<ProductDto>>
                .Success(cachedData, "Products retrieved from cache");
        }

        var query = _uow.Repository<Product>()
            .Query()
            .AsNoTracking()
            .Include(x => x.Category)
            .Include(x => x.ProductBatches)
            .Where(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var keyword = request.Keyword.Trim();

            query = query.Where(x =>
                x.Name.Contains(keyword) ||
                (x.ScientificName != null && x.ScientificName.Contains(keyword)) ||
                x.Barcode.Contains(keyword));
        }

        if (request.CategoryId.HasValue)
        {
            query = query.Where(x =>
                x.CategoryId == request.CategoryId.Value);
        }

        if (request.IsLowStock == true)
        {
            query = query.Where(x =>
                (x.ProductBatches.Sum(b => (int?)b.Quantity) ?? 0)
                <= x.MinStockLevel);
        }

        query = request.SortBy?.ToLower() switch
        {
            "price" => request.SortDirection == "desc"
                ? query.OrderByDescending(x => x.SalePrice)
                : query.OrderBy(x => x.SalePrice),

            "stock" => request.SortDirection == "desc"
                ? query.OrderByDescending(x =>
                    x.ProductBatches.Sum(b => (int?)b.Quantity) ?? 0)
                : query.OrderBy(x =>
                    x.ProductBatches.Sum(b => (int?)b.Quantity) ?? 0),

            "name" => request.SortDirection == "desc"
                ? query.OrderByDescending(x => x.Name)
                : query.OrderBy(x => x.Name),

            _ => query.OrderByDescending(x => x.CreatedAt)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var products = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Barcode = p.Barcode,
                SalePrice = p.SalePrice,

                TotalStock = p.ProductBatches.Sum(b => (int?)b.Quantity) ?? 0,

                IsLowStock =
                    (p.ProductBatches.Sum(b => (int?)b.Quantity) ?? 0)
                    <= p.MinStockLevel,

                Batches = p.ProductBatches.Select(b => new ProductBatchDto
                {
                    BatchNumber = b.BatchNumber,
                    Quantity = b.Quantity,
                    ExpiryDate = b.ExpiryDate
                }).ToList()
            })
            .ToListAsync(cancellationToken);

        var paginatedResult = new PaginatedResult<ProductDto>(
            products,
            totalCount,
            pageNumber,
            pageSize);

        await _cache.SetAsync(
            cacheKey,
            paginatedResult,
            TimeSpan.FromMinutes(10),
            cancellationToken);

        return Result<PaginatedResult<ProductDto>>
            .Success(paginatedResult, "Products retrieved successfully");
    }
}