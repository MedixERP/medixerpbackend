public class ProductFilterRequest
{
    public string? Keyword { get; set; }

    public int? CategoryId { get; set; }

    public bool? IsLowStock { get; set; }

    public string? SortBy { get; set; } = "name";

    public string? SortDirection { get; set; } = "asc";

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}