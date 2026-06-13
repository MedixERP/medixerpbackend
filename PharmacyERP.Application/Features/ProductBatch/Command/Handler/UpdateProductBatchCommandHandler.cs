using MediatR;
using Microsoft.AspNetCore.Http;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;

public class UpdateProductBatchCommandHandler
    : IRequestHandler<UpdateProductBatchCommand, Result<Unit>>
{
    private readonly IUnitOfWork _uow;
    private readonly IHttpContextAccessor _http;

    public UpdateProductBatchCommandHandler(IUnitOfWork uow, IHttpContextAccessor http)
    {
        _uow = uow;
        _http = http;
    }

    public async Task<Result<Unit>> Handle(
     UpdateProductBatchCommand request,
     CancellationToken cancellationToken)
    {
        var user = _http.HttpContext?.User;

        if (user == null || !user.Identity!.IsAuthenticated)
            return Result<Unit>.Failure("Unauthorized", 401);

        if (!user.IsInRole("Admin") && !user.IsInRole("Pharmacist"))
            return Result<Unit>.Failure("Forbidden", 403);

        var batch = await _uow.ProductBatches.GetByIdAsync(request.Id);

        if (batch == null || batch.IsDeleted)
            return Result<Unit>.Failure("Batch not found", 404);

        if (request.Quantity < 0)
            return Result<Unit>.Failure("Quantity cannot be negative", 400);

        if (request.ExpiryDate <= DateTime.UtcNow)
            return Result<Unit>.Failure("Expiry date must be in future", 400);

        if (request.PurchasePrice <= 0)
            return Result<Unit>.Failure("Invalid purchase price", 400);

        batch.Quantity = request.Quantity;
        batch.ExpiryDate = request.ExpiryDate;
        batch.PurchasePrice = request.PurchasePrice;
        batch.UpdatedAt = DateTime.UtcNow;

        _uow.ProductBatches.Update(batch);

        await _uow.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}