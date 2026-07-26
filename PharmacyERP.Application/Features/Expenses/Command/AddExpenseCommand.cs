using MediatR;
using PharmacyERP.Application.Common.Models;

public class AddExpenseCommand : IRequest<Result<int>>
{
    public string Title { get; set; }
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaidAt { get; set; } = DateTime.UtcNow;
}