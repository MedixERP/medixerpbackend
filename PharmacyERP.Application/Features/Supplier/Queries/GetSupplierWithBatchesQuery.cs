using MediatR;
using PharmacyERP.Application.Common.Models;

public class GetSupplierWithBatchesQuery
    : IRequest<Result<SupplierWithBatchesDto>>
{
    public int Id { get; set; }
}