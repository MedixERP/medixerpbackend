using MediatR;
using PharmacyERP.Application.Common.Models;

public class GetCustomerByIdQuery
    : IRequest<Result<CustomerDto>>
{
    public int Id { get; set; }
}