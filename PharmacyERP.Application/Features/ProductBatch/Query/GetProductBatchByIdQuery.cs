using MediatR;
using PharmacyERP.Application.Common.Models;

public class GetProductBatchByIdQuery
    : IRequest<Result<ProductBatchsDto>>
{
    public int Id { get; set; }
}