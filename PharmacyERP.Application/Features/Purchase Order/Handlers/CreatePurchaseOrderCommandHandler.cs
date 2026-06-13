using MediatR;
using Microsoft.AspNetCore.Http;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;
using PharmacyERP.Domain.Enums;
using System.Security.Claims;

public class CreatePurchaseOrderCommandHandler
    : IRequestHandler<CreatePurchaseOrderCommand, Result<int>>
{
    private readonly IUnitOfWork _uow;
    private readonly IHttpContextAccessor _http;

    public CreatePurchaseOrderCommandHandler(IUnitOfWork uow, IHttpContextAccessor http)
    {
        _uow = uow;
        _http = http;
    }

    public async Task<Result<int>> Handle(
      CreatePurchaseOrderCommand request,
      CancellationToken cancellationToken)
    {
        var httpContext = _http.HttpContext;

        if (httpContext == null)
            return Result<int>.Failure("HttpContext is null", 500);

        var user = httpContext.User;

        if (user == null || !user.Identity!.IsAuthenticated)
            return Result<int>.Failure("Unauthorized", 401);

        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            return Result<int>.Failure("Invalid user id", 400);

        if (!user.IsInRole("Admin") && !user.IsInRole("Pharmacist"))
            return Result<int>.Failure("Forbidden", 403);

        var supplier = await _uow.Repository<Supplier>()
            .GetByIdAsync(request.SupplierId);

        if (supplier == null)
            return Result<int>.Failure("Supplier not found", 404);

        if (request.Items == null || !request.Items.Any())
            return Result<int>.Failure("Order items are required", 400);

        decimal total = 0;

        var items = new List<PurchaseOrderItem>();

        foreach (var item in request.Items)
        {
            if (item.Quantity <= 0)
                return Result<int>.Failure("Invalid quantity", 400);

            if (item.UnitPrice <= 0)
                return Result<int>.Failure("Invalid unit price", 400);

            var product = await _uow.Products
                .GetByIdAsync(item.ProductId);

            if (product == null)
                return Result<int>.Failure($"Product {item.ProductId} not found", 404);

            total += item.Quantity * item.UnitPrice;

            items.Add(new PurchaseOrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            });
        }

        var order = new PurchaseOrder
        {
            SupplierId = request.SupplierId,
            CreatedByUserId = userId,

            OrderNumber = $"PO-{DateTime.UtcNow:yyyyMMddHHmmss}",

            Status = PurchaseOrderStatus.Pending,
            CreatedAt = DateTime.UtcNow,

            PurchaseOrderItems = items
        };

        await _uow.Repository<PurchaseOrder>().AddAsync(order);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(
            order.Id,
            $"Purchase order created successfully (Total: {total})");
    }
}