using MediatR;
using PharmacyERP.Application.Common.Models;

public class GetSalesReturnByIdQuery : IRequest<Result<SalesReturnDto>>
{
    public int Id { get; set; }
}