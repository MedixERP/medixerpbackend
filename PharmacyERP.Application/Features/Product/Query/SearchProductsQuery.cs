using MediatR;
using PharmacyERP.Application.Common.Models;

public class SearchProductsQuery
    : IRequest<Result<List<ProductSearchDto>>>
{
    public string Keyword { get; set; }
}