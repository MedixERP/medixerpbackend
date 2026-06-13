using MediatR;
using Microsoft.AspNetCore.Http;
using PharmacyERP.Application.Common.Models;

public class UpdateCustomerCommandHandler
    : IRequestHandler<UpdateCustomerCommand, Result<string>>
{
    private readonly IUnitOfWork _uow;
    private readonly IHttpContextAccessor _http;

    public UpdateCustomerCommandHandler(
        IUnitOfWork uow,
        IHttpContextAccessor http)
    {
        _uow = uow;
        _http = http;
    }

    public async Task<Result<string>> Handle(
        UpdateCustomerCommand request,
        CancellationToken cancellationToken)
    {
        var user = _http.HttpContext?.User;

        if (user == null || !user.Identity!.IsAuthenticated)
            return Result<string>.Failure("Unauthorized", 401);

        if (!user.IsInRole("Admin") && !user.IsInRole("Cashier"))
            return Result<string>.Failure("Forbidden", 403);

        var customer = await _uow.Customers.GetByIdAsync(request.Id);

        if (customer == null || customer.IsDeleted)
            return Result<string>.Failure("Customer not found", 404);

        if (string.IsNullOrWhiteSpace(request.FullName))
            return Result<string>.Failure("FullName is required", 400);

        if (string.IsNullOrWhiteSpace(request.Phone))
            return Result<string>.Failure("Phone is required", 400);

        var phone = request.Phone.Trim();

        var phoneExists = await _uow.Customers.IsPhoneExistsAsync(phone);

        if (phoneExists && customer.Phone != phone)
            return Result<string>.Failure("Phone already exists", 400);

        customer.FullName = request.FullName.Trim();
        customer.Phone = phone;
        customer.Address = request.Address?.Trim();
        customer.IsVip = request.IsVip;
        customer.CreditLimit = request.CreditLimit;
        customer.UpdatedAt = DateTime.UtcNow;

        _uow.Customers.Update(customer);

        await _uow.SaveChangesAsync(cancellationToken);

        return Result<string>.Success("Updated", "Customer updated successfully");
    }
}