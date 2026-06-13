using MediatR;
using PharmacyERP.Application.Common.Models;

public class AddSupplierCommand : IRequest<Result<int>>
{
    public string Name { get; set; }

    public string Phone { get; set; }

    public string Email { get; set; }

    public string Address { get; set; }
}