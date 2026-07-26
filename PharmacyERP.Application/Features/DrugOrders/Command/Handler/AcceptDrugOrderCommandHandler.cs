using MediatR;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;
using PharmacyERP.Domain.Enums;

public class AcceptDrugOrderCommandHandler
    : IRequestHandler<AcceptDrugOrderCommand, Result<string>>
{
    private readonly IUnitOfWork _uow;

    public AcceptDrugOrderCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<string>> Handle(
        AcceptDrugOrderCommand request,
        CancellationToken cancellationToken)
    {
        var order = await _uow.Repository<DrugOrder>()
            .GetByIdAsync(request.OrderId);

        if (order == null || order.IsDeleted)
            return Result<string>.Failure("Order not found", 404);

        if (order.Status != DrugOrderStatus.Pending)
            return Result<string>.Failure(
                "Only pending orders can be accepted", 400);

        order.Status = DrugOrderStatus.Approved;
        order.UpdatedAt = DateTime.UtcNow;

        _uow.Repository<DrugOrder>().Update(order);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result<string>.Success(
            "Accepted", "Order accepted successfully");
    }
}