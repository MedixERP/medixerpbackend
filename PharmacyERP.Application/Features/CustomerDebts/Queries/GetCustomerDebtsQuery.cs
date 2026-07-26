using MediatR;
using PharmacyERP.Application.Common.Models;

public class GetCustomerDebtsQuery : IRequest<Result<List<CustomerDebtDto>>>
{
    public int CustomerId { get; set; }
}