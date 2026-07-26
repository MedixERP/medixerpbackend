using MediatR;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Application.Features.SupplierPayments.Queries;
using PharmacyERP.Domain.Entities;

public class GetSupplierDebtQueryHandler
    : IRequestHandler<GetSupplierDebtQuery, Result<List<SupplierDebtDto>>>
{
    private readonly IUnitOfWork _uow;

    public GetSupplierDebtQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<List<SupplierDebtDto>>> Handle(
        GetSupplierDebtQuery request,
        CancellationToken cancellationToken)
    {
        var orders = await _uow.Repository<PurchaseOrder>()
            .Query()
            .Where(x => x.SupplierId == request.SupplierId)
            .ToListAsync(cancellationToken);

        var payments = await _uow.Repository<SupplierPayment>()
            .Query()
            .Where(x => x.SupplierId == request.SupplierId)
            .ToListAsync(cancellationToken);

        var result = orders.Select(o => new SupplierDebtDto
        {
            SupplierId = o.SupplierId,
            PurchaseOrderId = o.Id,
            TotalAmount = o.TotalAmount,
            TotalPaid = payments
                .Where(p => p.PurchaseOrderId == o.Id)
                .Sum(p => p.Amount),
            Remaining = o.TotalAmount - payments
                .Where(p => p.PurchaseOrderId == o.Id)
                .Sum(p => p.Amount)
        }).ToList();

        return Result<List<SupplierDebtDto>>.Success(
            result, "Debts retrieved successfully");
    }
}