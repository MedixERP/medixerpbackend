using MediatR;
using Microsoft.AspNetCore.Http;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;
using PharmacyERP.Domain.Enums;
using System.Security.Claims;

public class CheckoutCommandHandler
    : IRequestHandler<CheckoutCommand, Result<CheckoutResponseDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IHttpContextAccessor _http;
    private readonly ICartService _cart;

    public CheckoutCommandHandler(
        IUnitOfWork uow,
        IHttpContextAccessor http,
        ICartService cart)
    {
        _uow = uow;
        _http = http;
        _cart = cart;
    }

    public async Task<Result<CheckoutResponseDto>> Handle(
        CheckoutCommand request,
        CancellationToken cancellationToken)
    {
        var user = _http.HttpContext?.User;

        if (user?.Identity?.IsAuthenticated != true)
            return Result<CheckoutResponseDto>.Failure("Unauthorized", 401);

        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(userIdClaim))
            return Result<CheckoutResponseDto>.Failure("UserId missing", 400);

        var userId = int.Parse(userIdClaim);

        var cart = _cart.GetCart(userIdClaim);

        if (cart.Count == 0)
            return Result<CheckoutResponseDto>.Failure("Cart is empty", 400);

        using var trx = await _uow.BeginTransactionAsync();

        try
        {
            decimal total = 0;

            var invoice = new Invoice
            {
                CreatedByUserId = userId,
                CustomerId = request.CustomerId ?? 0,
                InvoiceNumber = $"INV-{DateTime.UtcNow.Ticks}",
                CreatedAt = DateTime.UtcNow,
                InvoiceItems = new List<InvoiceItem>(),
                IsCancelled = false
            };

            var movements = new List<InventoryMovement>();

            foreach (var item in cart)
            {
                var batch = await _uow.ProductBatches.GetByIdAsync(item.BatchId);

                if (batch == null)
                    return Result<CheckoutResponseDto>.Failure("Batch not found", 404);

                if (batch.Quantity < item.Quantity)
                    return Result<CheckoutResponseDto>.Failure("Insufficient stock", 400);

                var product = await _uow.Products.GetByIdAsync(item.ProductId);

                if (product == null)
                    return Result<CheckoutResponseDto>.Failure("Product not found", 404);

                var beforeQty = batch.Quantity;

                batch.Quantity -= item.Quantity;

                total += item.UnitPrice * item.Quantity;

                invoice.InvoiceItems.Add(new InvoiceItem
                {
                    ProductId = item.ProductId,
                    BatchId = item.BatchId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    Total = item.UnitPrice * item.Quantity
                });

                movements.Add(new InventoryMovement
                {
                    ProductId = item.ProductId,
                    BatchId = item.BatchId,
                    Quantity = item.Quantity,
                    Type = InventoryMovementType.Sale,
                    Reason = "POS Checkout",
                    UserId = userId,
                    BeforeQuantity = beforeQty,
                    AfterQuantity = batch.Quantity,
                    CreatedAt = DateTime.UtcNow
                });
            }

            var discount = request.Discount;
            var paid = request.PaidAmount;

            invoice.TotalAmount = total;
            invoice.FinalAmount = Math.Max(0, total - discount);

            var change = paid - invoice.FinalAmount;

            await _uow.Repository<Invoice>().AddAsync(invoice);
            await _uow.SaveChangesAsync(cancellationToken);

            foreach (var m in movements)
            {
                m.ReferenceId = invoice.Id;
                await _uow.Repository<InventoryMovement>().AddAsync(m);
            }

            await _uow.SaveChangesAsync(cancellationToken);

            _cart.Clear(userIdClaim);

            await trx.CommitAsync();

            return Result<CheckoutResponseDto>.Success(new CheckoutResponseDto
            {
                InvoiceId = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                Total = invoice.TotalAmount,
                FinalTotal = invoice.FinalAmount,
                Change = change,
                PdfUrl = $"/api/invoices/{invoice.Id}/pdf"
            }, "Checkout completed");
        }
        catch
        {
            await trx.RollbackAsync();
            throw;
        }
    }
}