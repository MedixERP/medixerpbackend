using MediatR;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Application.Features.SupplierPayments.Queries;

public class GetSupplierDebtQuery : IRequest<Result<List<SupplierDebtDto>>>
{
    public int SupplierId { get; set; }
}