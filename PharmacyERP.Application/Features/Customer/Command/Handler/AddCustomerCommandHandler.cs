using Application.Common.Interfaces;
using MediatR;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;

public class AddCustomerCommandHandler
    : IRequestHandler<AddCustomerCommand, Result<int>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICacheService _cache;

    public AddCustomerCommandHandler(IUnitOfWork uow, ICacheService cache)
    {
        _uow = uow;
        _cache = cache;
    }

    public async Task<Result<int>> Handle(
        AddCustomerCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.FullName))
            return Result<int>.Failure("FullName is required", 400);

        if (string.IsNullOrWhiteSpace(request.Phone))
            return Result<int>.Failure("Phone is required", 400);

        var phone = request.Phone.Trim();

        var exists = await _uow.Customers.IsPhoneExistsAsync(phone);
        if (exists)
            return Result<int>.Failure("Phone already exists", 400);

        var customer = new Customer
        {
            FullName = request.FullName.Trim(),
            Phone = phone,
            Address = request.Address?.Trim(),
            IsVip = request.IsVip,
            CreditLimit = request.CreditLimit,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await _uow.Customers.AddAsync(customer);
        await _uow.SaveChangesAsync(cancellationToken);

        await _cache.RemoveByPatternAsync("customers:*", cancellationToken);

        return Result<int>.Success(customer.Id, "Customer added successfully");
    }
}