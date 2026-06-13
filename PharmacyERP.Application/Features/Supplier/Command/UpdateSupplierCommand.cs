using MediatR;
using PharmacyERP.Application.Common.Models;

using MediatRUnit = MediatR.Unit;

public class UpdateSupplierCommand
    : IRequest<Result<MediatRUnit>>
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Phone { get; set; }

    public string Address { get; set; }
}