using MediatR;
using PharmacyERP.Application.Common.Models;

using MediatRUnit = MediatR.Unit;

public class DeleteSupplierCommand
    : IRequest<Result<MediatRUnit>>
{
    public int Id { get; set; }
}