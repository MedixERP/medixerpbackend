using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;
using PharmacyERP.Domain.Enums;
using System.Security.Claims;

public class AddInvoicePaymentCommandHandler
    : IRequestHandler<AddInvoicePaymentCommand, Result<CustomerDebtDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IHttpContextAccessor _http;

    public AddInvoicePaymentCommandHandler(
        IUnitOfWork uow,
        IHttpContextAccessor http)
    {
        _uow = uow;
        _http = http;
    }

    public async Task<Result<CustomerDebtDto>> Handle(
        AddInvoicePaymentCommand request,
        CancellationToken cancellationToken)
    {
        var invoice = await _uow.Repository<Invoice>()
            .Query()
            .FirstOrDefaultAsync(
                x => x.Id == request.InvoiceId,
                cancellationToken);

        if (invoice == null)
            return Result<CustomerDebtDto>.Failure(
                "Invoice not found", 404);

        if (invoice.IsCancelled)
            return Result<CustomerDebtDto>.Failure(
                "Cannot pay a cancelled invoice", 400);

        var alreadyPaid = await _uow.Repository<Payment>()
            .Query()
            .Where(x => x.InvoiceId == request.InvoiceId)
            .SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0;

        var remaining = invoice.FinalAmount - alreadyPaid;

        if (request.Amount > remaining)
            return Result<CustomerDebtDto>.Failure(
                $"Payment exceeds remaining debt ({remaining:0.00})", 400);

        var userIdClaim = _http.HttpContext?.User?
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userId = userIdClaim != null ? int.Parse(userIdClaim) : 0;

        var payment = new Payment
        {
            InvoiceId = request.InvoiceId,
            Amount = request.Amount,
            PaymentMethod = request.PaymentMethod,
            PaidAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.Repository<Payment>().AddAsync(payment);

        var totalPaidAfter = alreadyPaid + request.Amount;
        invoice.PaymentStatus = totalPaidAfter >= invoice.FinalAmount
            ? PaymentStatus.Paid
            : PaymentStatus.Pending;

        _uow.Repository<Invoice>().Update(invoice);

        var cashbox = new CashboxTransaction
        {
            Type = CashboxTransactionType.In,
            Source = CashboxSource.CustomerPayment,
            Amount = request.Amount,
            ReferenceType = "Invoice",
            ReferenceId = invoice.Id,
            Description = $"Payment for invoice {invoice.InvoiceNumber}",
            CreatedByUserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.Repository<CashboxTransaction>().AddAsync(cashbox);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result<CustomerDebtDto>.Success(
            new CustomerDebtDto
            {
                CustomerId = invoice.CustomerId,
                InvoiceId = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                TotalAmount = invoice.FinalAmount,
                TotalPaid = totalPaidAfter,
                Remaining = invoice.FinalAmount - totalPaidAfter
            },
            "Payment recorded successfully");
    }
}