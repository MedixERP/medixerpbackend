using MediatR;

public class GetSalesSummaryQuery : IRequest<SalesSummaryDto>
{
    public DateTime Date { get; set; }
}