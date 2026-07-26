using MediatR;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;
using PharmacyERP.Domain.Enums;

public class UpdateDrugOrderStatusCommandHandler
    : IRequestHandler<UpdateDrugOrderStatusCommand, Result<string>>
{
    private readonly IUnitOfWork _uow;

    public UpdateDrugOrderStatusCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<string>> Handle(
        UpdateDrugOrderStatusCommand request,
        CancellationToken cancellationToken)
    {
        var order = await _uow.Repository<DrugOrder>()
            .GetByIdAsync(request.OrderId);

        if (order == null || order.IsDeleted)
            return Result<string>.Failure("Order not found", 404);

        if (!Enum.TryParse<DrugOrderStatus>(
            request.Status, true, out var newStatus))
            return Result<string>.Failure(
                "Invalid status. Valid values: Pending, Approved, Preparing, Shipped, Delivered, Rejected, Completed",
                400);

        order.Status = newStatus;
        order.UpdatedAt = DateTime.UtcNow;

        _uow.Repository<DrugOrder>().Update(order);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result<string>.Success(
            "Updated", $"Order status updated to {newStatus}");
    }
}