public class CashboxBalanceDto
{
    public decimal TotalIn { get; set; }
    public decimal TotalOut { get; set; }
    public decimal Balance { get; set; }
}

public class CashboxTransactionDto
{
    public int Id { get; set; }
    public string Type { get; set; }
    public string Source { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}