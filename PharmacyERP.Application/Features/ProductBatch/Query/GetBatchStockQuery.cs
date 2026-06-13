using MediatR;
using PharmacyERP.Application.Common.Models;

public class GetBatchStockQuery : IRequest<Result<int>>
{
    public int ProductId { get; set; }
}