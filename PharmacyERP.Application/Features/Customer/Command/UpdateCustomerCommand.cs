using MediatR;
using PharmacyERP.Application.Common.Models;

public class UpdateCustomerCommand : IRequest<Result<string>>
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public bool IsVip { get; set; }
    public decimal CreditLimit { get; set; }
}