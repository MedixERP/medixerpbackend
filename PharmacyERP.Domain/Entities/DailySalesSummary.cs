
namespace PharmacyERP.Domain.Entities;

public class DailySalesSummary : BaseEntity
{
    public DateTime Date { get; set; }

    public decimal TotalSales { get; set; }

    public int TotalInvoices { get; set; }
}