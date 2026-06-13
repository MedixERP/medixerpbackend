using MediatR;

public class GetSalesReturnsQuery : IRequest<List<SalesReturnDto>>
{
    public int? InvoiceId { get; set; }
    public int? UserId { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
}
