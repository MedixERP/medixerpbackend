using MediatR;
using Microsoft.AspNetCore.Http;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;

public class AddProductBatchCommandHandler
    : IRequestHandler<AddProductBatchCommand, Result<int>>
{
    private readonly IUnitOfWork _uow;
    private readonly IHttpContextAccessor _http;

    public AddProductBatchCommandHandler(IUnitOfWork uow, IHttpContextAccessor http)
    {
        _uow = uow;
        _http = http;
    }

    public async Task<Result<int>> Handle(
      AddProductBatchCommand request,
      CancellationToken cancellationToken)
    {
        var user = _http.HttpContext?.User;

        if (user == null || !user.Identity!.IsAuthenticated)
            return Result<int>.Failure("Unauthorized", 401);

        if (!user.IsInRole("Admin") && !user.IsInRole("Pharmacist"))
            return Result<int>.Failure("Forbidden", 403);

        var product = await _uow.Products.GetByIdAsync(request.ProductId);
        if (product == null)
            return Result<int>.Failure("Product not found", 404);

        var supplier = await _uow.Repository<Supplier>()
            .GetByIdAsync(request.SupplierId);

        if (supplier == null)
            return Result<int>.Failure("Supplier not found", 404);

        if (request.Quantity <= 0)
            return Result<int>.Failure("Invalid quantity", 400);

        var batch = new ProductBatch
        {
            ProductId = request.ProductId,
            BatchNumber = request.BatchNumber.Trim(),
            Quantity = request.Quantity,
            ExpiryDate = request.ExpiryDate,
            SupplierId = request.SupplierId,
            PurchasePrice = request.PurchasePrice,
            ReceivedDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await _uow.ProductBatches.AddAsync(batch);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(batch.Id);
    }
}