using MediatR;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;

public class GetDrugOrderByIdHandler
    : IRequestHandler<GetDrugOrderByIdQuery, Result<DrugOrderDto>>
{
    private readonly IUnitOfWork _uow;

    public GetDrugOrderByIdHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<DrugOrderDto>> Handle(
        GetDrugOrderByIdQuery request,
        CancellationToken cancellationToken)
    {
        var order = await _uow.Repository<DrugOrder>()
            .Query()
            .AsNoTracking()
            .Include(x => x.PharmacyCompany)
            .Include(x => x.CreatedByUser)
            .Include(x => x.Items)
                .ThenInclude(i => i.Product)
            .Where(x => x.Id == request.Id && !x.IsDeleted)
            .Select(x => new DrugOrderDto
            {
                Id = x.Id,
                OrderNumber = x.OrderNumber,
                CompanyName = x.PharmacyCompany.Name,
                CreatedBy = x.CreatedByUser.FullName,
                Status = x.Status.ToString(),
                RejectionReason = x.RejectionReason,
                SupplierName = x.SupplierName,
                SupplierPhone = x.SupplierPhone,
                TotalAmount = x.TotalAmount,
                ReceivedAt = x.ReceivedAt,
                CreatedAt = x.CreatedAt,
                Items = x.Items.Select(i => new DrugOrderItemDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product.Name,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    Total = i.Total
                }).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (order == null)
            return Result<DrugOrderDto>.Failure("Order not found", 404);

        return Result<DrugOrderDto>.Success(
            order, "Order retrieved successfully");
    }
}