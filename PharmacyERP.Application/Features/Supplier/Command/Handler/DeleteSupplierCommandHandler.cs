using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;

using MediatRUnit = MediatR.Unit;

public class DeleteSupplierCommandHandler
    : IRequestHandler<
        DeleteSupplierCommand,
        Result<MediatRUnit>>
{
    private readonly IUnitOfWork _uow;
    private readonly IHttpContextAccessor _http;

    public DeleteSupplierCommandHandler(
        IUnitOfWork uow,
        IHttpContextAccessor http)
    {
        _uow = uow;
        _http = http;
    }

    public async Task<Result<MediatRUnit>> Handle(
        DeleteSupplierCommand request,
        CancellationToken cancellationToken)
    {
        var user = _http.HttpContext?.User;

        if (user == null || !user.Identity!.IsAuthenticated)
            return Result<MediatRUnit>.Failure("Unauthorized", 401);

        if (!user.IsInRole("Admin"))
            return Result<MediatRUnit>.Failure("Only Admin can delete supplier", 403);

        var supplier = await _uow.Suppliers
     .GetByIdAsync(request.Id);

        if (supplier == null || supplier.IsDeleted)
            return Result<MediatRUnit>.Failure("Supplier not found", 404);

        var hasBatches = await _uow.Repository<ProductBatch>()
            .Query()
            .AnyAsync(x => x.SupplierId == supplier.Id);

        var hasOrders = await _uow.Repository<PurchaseOrder>()
            .Query()
            .AnyAsync(x => x.SupplierId == supplier.Id);

        if (hasBatches || hasOrders)
            return Result<MediatRUnit>.Failure(
                "Cannot delete supplier (linked data exists)",
                400);

        supplier.IsDeleted = true;
        supplier.UpdatedAt = DateTime.UtcNow;

        _uow.Suppliers.Update(supplier);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result<MediatRUnit>.Success(MediatRUnit.Value);
    }
}