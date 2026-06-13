using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Models;

namespace PharmacyERP.Application.Common.Extensions;

public static class QueryableExtensions
{
    public static async Task<PaginatedResult<T>>
        ToPaginatedListAsync<T>(
            this IQueryable<T> query,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
    {
        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<T>(
            items,
            totalCount,
            pageNumber,
            pageSize);
    }
}