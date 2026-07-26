namespace PharmacyERP.Domain.Enums;

public enum DrugOrderStatus
{
    Pending = 1,
    Approved = 2,
    Preparing = 3,
    Shipped = 4,
    Delivered = 5,
    Rejected = 6,
    Completed = 7
}