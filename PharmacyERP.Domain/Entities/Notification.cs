using PharmacyERP.Domain.Enums;


namespace PharmacyERP.Domain.Entities;

public class Notification : BaseEntity
{
    public string Title { get; set; }

    public string Message { get; set; }

    public NotificationType Type { get; set; }

    public bool IsRead { get; set; }

    public int UserId { get; set; }

    public ApplicationUser User { get; set; }
}