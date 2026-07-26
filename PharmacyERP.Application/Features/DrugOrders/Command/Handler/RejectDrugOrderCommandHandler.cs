using MediatR;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;
using PharmacyERP.Domain.Enums;

public class RejectDrugOrderCommandHandler
    : IRequestHandler<RejectDrugOrderCommand, Result<string>>
{
    private readonly IUnitOfWork _uow;

    public RejectDrugOrderCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<string>> Handle(
        RejectDrugOrderCommand request,
        CancellationToken cancellationToken)
    {
        var order = await _uow.Repository<DrugOrder>()
            .GetByIdAsync(request.OrderId);

        if (order == null || order.IsDeleted)
            return Result<string>.Failure("Order not found", 404);

        if (order.Status != DrugOrderStatus.Pending)
            return Result<string>.Failure(
                "Only pending orders can be rejected", 400);

        order.Status = DrugOrderStatus.Rejected;
        order.RejectionReason = request.Reason?.Trim();
        order.UpdatedAt = DateTime.UtcNow;

        _uow.Repository<DrugOrder>().Update(order);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result<string>.Success(
            "Rejected", "Order rejected successfully");
    }
}