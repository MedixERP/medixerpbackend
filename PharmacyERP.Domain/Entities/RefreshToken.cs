
namespace PharmacyERP.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public int UserId { get; set; }

    public string Token { get; set; }

    public DateTime ExpiresAt { get; set; }

    public bool IsRevoked { get; set; }

    public ApplicationUser User { get; set; }
}