namespace PharmacyERP.Application.Common.Models;

public class SortingRequest
{
    public string? SortBy { get; set; }

    public bool Descending { get; set; } = false;
}