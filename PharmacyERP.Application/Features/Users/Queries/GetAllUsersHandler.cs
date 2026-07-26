using Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;

public class GetAllUsersHandler
    : IRequestHandler<GetAllUsersQuery, Result<PaginatedResult<UserDto>>>
{
    private readonly IUnitOfWork _uow;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICacheService _cache;

    public GetAllUsersHandler(
        IUnitOfWork uow,
        UserManager<ApplicationUser> userManager,
        ICacheService cache)
    {
        _uow = uow;
        _userManager = userManager;
        _cache = cache;
    }

    public async Task<Result<PaginatedResult<UserDto>>> Handle(
        GetAllUsersQuery request,
        CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;

        var cacheKey =
            $"users:{pageNumber}:{pageSize}:{request.Keyword}:{request.IsActive}:{request.Role}";

        var cachedData =
            await _cache.GetAsync<PaginatedResult<UserDto>>(cacheKey, cancellationToken);

        if (cachedData is not null)
        {
            return Result<PaginatedResult<UserDto>>
                .Success(cachedData, "Users retrieved from cache");
        }

        var query = _uow.Repository<ApplicationUser>()
            .Query()
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var keyword = request.Keyword.Trim().ToLower();
            query = query.Where(x =>
                x.FullName.ToLower().Contains(keyword) ||
                (x.Email != null && x.Email.ToLower().Contains(keyword)));
        }

        if (request.IsActive.HasValue)
            query = query.Where(x => x.IsActive == request.IsActive.Value);

        var users = await query
            .OrderByDescending(x => x.Id)
            .ToListAsync(cancellationToken);

        var allDtos = new List<UserDto>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            if (!string.IsNullOrWhiteSpace(request.Role) &&
                !roles.Contains(request.Role, StringComparer.OrdinalIgnoreCase))
                continue;

            allDtos.Add(new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,
                Roles = roles.ToList()
            });
        }

        var totalCount = allDtos.Count;

        var pagedData = allDtos
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var result = new PaginatedResult<UserDto>(pagedData, totalCount, pageNumber, pageSize);

        await _cache.SetAsync(
            cacheKey,
            result,
            TimeSpan.FromMinutes(5),
            cancellationToken);

        return Result<PaginatedResult<UserDto>>.Success(
            result,
            "Users retrieved successfully");
    }
}