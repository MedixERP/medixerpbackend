using MediatR;
using PharmacyERP.Application.Common.Models;

public class GetProductByIdQuery
    : IRequest<Result<ProductDto>>
{
    public int Id { get; set; }
}