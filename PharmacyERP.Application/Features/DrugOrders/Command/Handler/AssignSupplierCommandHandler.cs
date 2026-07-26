using MediatR;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;
using PharmacyERP.Domain.Enums;

public class AssignSupplierCommandHandler
    : IRequestHandler<AssignSupplierCommand, Result<string>>
{
    private readonly IUnitOfWork _uow;

    public AssignSupplierCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<string>> Handle(
        AssignSupplierCommand request,
        CancellationToken cancellationToken)
    {
        var order = await _uow.Repository<DrugOrder>()
            .GetByIdAsync(request.OrderId);

        if (order == null || order.IsDeleted)
            return Result<string>.Failure("Order not found", 404);

        if (order.Status != DrugOrderStatus.Approved)
            return Result<string>.Failure(
                "Order must be approved before assigning supplier", 400);

        order.SupplierName = request.SupplierName.Trim();
        order.SupplierPhone = request.SupplierPhone.Trim();
        order.Status = DrugOrderStatus.Preparing;
        order.UpdatedAt = DateTime.UtcNow;

        _uow.Repository<DrugOrder>().Update(order);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result<string>.Success(
            "Assigned", "Supplier assigned and order is now preparing");
    }
}