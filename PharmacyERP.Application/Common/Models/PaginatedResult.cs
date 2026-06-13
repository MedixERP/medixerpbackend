namespace PharmacyERP.Application.Common.Models;

public class PaginatedResult<T>
{
    public IReadOnlyList<T> Data { get; set; } = [];

    public int PageNumber { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    public int TotalPages { get; set; }

    public bool HasPreviousPage => PageNumber > 1;

    public bool HasNextPage => PageNumber < TotalPages;

    public PaginatedResult() { }

    public PaginatedResult(
        IReadOnlyList<T> data,
        int totalCount,
        int pageNumber,
        int pageSize)
    {
        Data = data;
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;

        TotalPages = pageSize <= 0
            ? 0
            : (int)Math.Ceiling(totalCount / (double)pageSize);

        if (TotalPages == 0 && totalCount > 0)
            TotalPages = 1;
    }
}