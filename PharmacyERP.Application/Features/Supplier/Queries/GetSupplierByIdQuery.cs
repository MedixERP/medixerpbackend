using MediatR;
using PharmacyERP.Application.Common.Models;

public class GetSupplierByIdQuery : IRequest<Result<SupplierDto>>
{
    public int Id { get; set; }
}