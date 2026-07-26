using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;
using PharmacyERP.Domain.Enums;
using System.Security.Claims;

public class CreateDrugOrderCommandHandler
    : IRequestHandler<CreateDrugOrderCommand, Result<int>>
{
    private readonly IUnitOfWork _uow;
    private readonly IHttpContextAccessor _http;

    public CreateDrugOrderCommandHandler(
        IUnitOfWork uow,
        IHttpContextAccessor http)
    {
        _uow = uow;
        _http = http;
    }

    public async Task<Result<int>> Handle(
        CreateDrugOrderCommand request,
        CancellationToken cancellationToken)
    {
        var company = await _uow.Repository<PharmacyCompany>()
            .GetByIdAsync(request.PharmacyCompanyId);

        if (company == null || company.IsDeleted)
            return Result<int>.Failure("Company not found", 404);

        if (!company.IsActive)
            return Result<int>.Failure(
                "Cannot create order for inactive company", 400);

        var userIdClaim = _http.HttpContext?.User?
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userId = userIdClaim != null ? int.Parse(userIdClaim) : 0;

        var lastOrder = await _uow.Repository<DrugOrder>()
            .Query()
            .OrderByDescending(x => x.Id)
            .Select(x => new { x.Id })
            .FirstOrDefaultAsync(cancellationToken);

        var nextId = (lastOrder?.Id ?? 0) + 1;
        var orderNumber = $"DO-{nextId:D5}";

        var order = new DrugOrder
        {
            OrderNumber = orderNumber,
            PharmacyCompanyId = request.PharmacyCompanyId,
            CreatedByUserId = userId,
            Status = DrugOrderStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            Items = new List<DrugOrderItem>()
        };

        decimal total = 0;
        foreach (var item in request.Items)
        {
            var product = await _uow.Products.GetByIdAsync(item.ProductId);
            if (product == null || product.IsDeleted)
                return Result<int>.Failure(
                    $"Product {item.ProductId} not found", 404);

            var itemTotal = item.Quantity * item.UnitPrice;
            total += itemTotal;

            order.Items.Add(new DrugOrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                Total = itemTotal,
                CreatedAt = DateTime.UtcNow
            });
        }

        order.TotalAmount = total;

        await _uow.Repository<DrugOrder>().AddAsync(order);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(
            order.Id, "Drug order created successfully");
    }
}