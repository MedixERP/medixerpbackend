using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Application.Features.SupplierPayments.Command;
using PharmacyERP.Application.Features.SupplierPayments.Queries;
using PharmacyERP.Domain.Entities;
using PharmacyERP.Domain.Enums;
using System.Security.Claims;

public class AddSupplierPaymentCommandHandler
    : IRequestHandler<AddSupplierPaymentCommand, Result<SupplierDebtDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IHttpContextAccessor _http;

    public AddSupplierPaymentCommandHandler(
        IUnitOfWork uow,
        IHttpContextAccessor http)
    {
        _uow = uow;
        _http = http;
    }

    public async Task<Result<SupplierDebtDto>> Handle(
        AddSupplierPaymentCommand request,
        CancellationToken cancellationToken)
    {
        var order = await _uow.Repository<PurchaseOrder>()
            .Query()
            .FirstOrDefaultAsync(
                x => x.Id == request.PurchaseOrderId &&
                     x.SupplierId == request.SupplierId,
                cancellationToken);

        if (order == null)
            return Result<SupplierDebtDto>.Failure(
                "Purchase order not found for this supplier", 404);

        var alreadyPaid = await _uow.Repository<SupplierPayment>()
            .Query()
            .Where(x => x.PurchaseOrderId == request.PurchaseOrderId)
            .SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0;

        var remaining = order.TotalAmount - alreadyPaid;

        if (request.Amount > remaining)
            return Result<SupplierDebtDto>.Failure(
                $"Payment exceeds remaining debt ({remaining:0.00})", 400);

        var userIdClaim = _http.HttpContext?.User?
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userId = userIdClaim != null ? int.Parse(userIdClaim) : 0;

        var payment = new SupplierPayment
        {
            SupplierId = request.SupplierId,
            PurchaseOrderId = request.PurchaseOrderId,
            Amount = request.Amount,
            PaymentMethod = request.PaymentMethod,
            PaidAt = DateTime.UtcNow,
            CreatedByUserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.Repository<SupplierPayment>().AddAsync(payment);

        var cashbox = new CashboxTransaction
        {
            Type = CashboxTransactionType.Out,
            Source = CashboxSource.SupplierPayment,
            Amount = request.Amount,
            ReferenceType = "PurchaseOrder",
            ReferenceId = order.Id,
            Description = $"Payment to supplier - PO #{order.OrderNumber}",
            CreatedByUserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.Repository<CashboxTransaction>().AddAsync(cashbox);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result<SupplierDebtDto>.Success(
            new SupplierDebtDto
            {
                SupplierId = request.SupplierId,
                PurchaseOrderId = request.PurchaseOrderId,
                TotalAmount = order.TotalAmount,
                TotalPaid = alreadyPaid + request.Amount,
                Remaining = remaining - request.Amount
            },
            "Payment recorded successfully");
    }
}