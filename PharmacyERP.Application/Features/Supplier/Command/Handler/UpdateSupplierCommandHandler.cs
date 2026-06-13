using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;

using MediatRUnit = MediatR.Unit;

public class UpdateSupplierCommandHandler
    : IRequestHandler<
        UpdateSupplierCommand,
        Result<MediatRUnit>>
{
    private readonly IUnitOfWork _uow;
    private readonly IHttpContextAccessor _http;

    public UpdateSupplierCommandHandler(
        IUnitOfWork uow,
        IHttpContextAccessor http)
    {
        _uow = uow;
        _http = http;
    }

    public async Task<Result<MediatRUnit>> Handle(
        UpdateSupplierCommand request,
        CancellationToken cancellationToken)
    {
        var user = _http.HttpContext?.User;

        if (user == null || !user.Identity!.IsAuthenticated)
            return Result<MediatRUnit>.Failure("Unauthorized", 401);

        if (!user.IsInRole("Admin") &&
            !user.IsInRole("Pharmacist"))
        {
            return Result<MediatRUnit>.Failure("Forbidden", 403);
        }

        var supplier = await _uow.Suppliers.GetByIdAsync(request.Id);

        if (supplier == null || supplier.IsDeleted)
            return Result<MediatRUnit>.Failure("Supplier not found", 404);

        var exists = await _uow.Suppliers
            .Query()
            .AnyAsync(x =>
                x.Id != request.Id &&
                x.Name == request.Name.Trim());

        if (exists)
            return Result<MediatRUnit>.Failure("Supplier name already exists", 400);

        supplier.Name = request.Name.Trim();
        supplier.Phone = request.Phone.Trim();
        supplier.Address = request.Address.Trim();
        supplier.UpdatedAt = DateTime.UtcNow;

        _uow.Suppliers.Update(supplier);
        await _uow.SaveChangesAsync(cancellationToken);
        return Result<MediatRUnit>.Success(MediatRUnit.Value);
    }
}