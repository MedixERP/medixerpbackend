using MediatR;
using PharmacyERP.Application.Common.Models;

public class AddCustomerCommand : IRequest<Result<int>>
{
    public string FullName { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public bool IsVip { get; set; }
    public decimal CreditLimit { get; set; }
}