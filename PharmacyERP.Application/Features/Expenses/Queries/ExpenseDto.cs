public class ExpenseDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaidAt { get; set; }
    public string CreatedBy { get; set; }
}