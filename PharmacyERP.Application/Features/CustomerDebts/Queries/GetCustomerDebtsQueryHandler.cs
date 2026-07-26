using MediatR;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;

public class GetCustomerDebtsQueryHandler
    : IRequestHandler<GetCustomerDebtsQuery, Result<List<CustomerDebtDto>>>
{
    private readonly IUnitOfWork _uow;

    public GetCustomerDebtsQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<List<CustomerDebtDto>>> Handle(
        GetCustomerDebtsQuery request,
        CancellationToken cancellationToken)
    {
        var invoices = await _uow.Repository<Invoice>()
            .Query()
            .Where(x => x.CustomerId == request.CustomerId
                        && !x.IsCancelled)
            .ToListAsync(cancellationToken);

        var invoiceIds = invoices.Select(i => i.Id).ToList();

        var payments = await _uow.Repository<Payment>()
            .Query()
            .Where(x => invoiceIds.Contains(x.InvoiceId))
            .ToListAsync(cancellationToken);

        var result = invoices
            .Select(i => new CustomerDebtDto
            {
                CustomerId = request.CustomerId,
                InvoiceId = i.Id,
                InvoiceNumber = i.InvoiceNumber,
                TotalAmount = i.FinalAmount,
                TotalPaid = payments
                    .Where(p => p.InvoiceId == i.Id)
                    .Sum(p => p.Amount),
                Remaining = i.FinalAmount - payments
                    .Where(p => p.InvoiceId == i.Id)
                    .Sum(p => p.Amount)
            })
            .Where(x => x.Remaining > 0)
            .ToList();

        return Result<List<CustomerDebtDto>>.Success(
            result, "Debts retrieved successfully");
    }
}