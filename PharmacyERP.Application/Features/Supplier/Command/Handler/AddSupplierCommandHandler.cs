using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;

public class AddSupplierCommandHandler
    : IRequestHandler<AddSupplierCommand, Result<int>>
{
    private readonly IUnitOfWork _uow;
    private readonly IHttpContextAccessor _http;

    public AddSupplierCommandHandler(
        IUnitOfWork uow,
        IHttpContextAccessor http)
    {
        _uow = uow;
        _http = http;
    }

    public async Task<Result<int>> Handle(
     AddSupplierCommand request,
     CancellationToken cancellationToken)
    {
        var user = _http.HttpContext?.User;

        if (user == null || !user.Identity!.IsAuthenticated)
            return Result<int>.Failure("Unauthorized", 401);

        if (!user.IsInRole("Admin") && !user.IsInRole("Pharmacist"))
            return Result<int>.Failure("Forbidden", 403);

        if (string.IsNullOrWhiteSpace(request.Email))
            return Result<int>.Failure("Email is required", 400);

        var exists = await _uow.Suppliers
            .Query()
            .AnyAsync(x =>
                x.Name == request.Name.Trim() ||
                x.Email == request.Email.Trim());

        if (exists)
            return Result<int>.Failure("Supplier already exists", 400);

        var supplier = new Supplier
        {
            Name = request.Name.Trim(),
            Phone = request.Phone.Trim(),
            Address = request.Address.Trim(),
            Email = request.Email.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        await _uow.Suppliers.AddAsync(supplier);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(supplier.Id);
    }
}