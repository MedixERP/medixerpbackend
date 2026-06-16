namespace PharmacyERP.Domain.Entities;

public class UserSettings
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Language { get; set; } = "en";

    public string Theme { get; set; } = "light";

    public string? ProfileImageUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public ApplicationUser User { get; set; } = null!;
}