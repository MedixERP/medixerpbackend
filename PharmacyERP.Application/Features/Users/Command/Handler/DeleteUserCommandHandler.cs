using Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;
using System.Security.Claims;

public class DeleteUserCommandHandler
    : IRequestHandler<DeleteUserCommand, Result<string>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUnitOfWork _uow;
    private readonly IHttpContextAccessor _http;
    private readonly ICacheService _cache;

    public DeleteUserCommandHandler(
        UserManager<ApplicationUser> userManager,
        IUnitOfWork uow,
        IHttpContextAccessor http,
        ICacheService cache)
    {
        _userManager = userManager;
        _uow = uow;
        _http = http;
        _cache = cache;
    }

    public async Task<Result<string>> Handle(
        DeleteUserCommand request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _http.HttpContext?.User?
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (currentUserId != null && int.Parse(currentUserId) == request.Id)
            return Result<string>.Failure("You cannot delete your own account", 400);

        var user = await _userManager.FindByIdAsync(request.Id.ToString());
        if (user == null)
            return Result<string>.Failure("User not found", 404);

        var hasInvoices = await _uow.Repository<Invoice>()
            .Query()
            .AnyAsync(x => x.CreatedByUserId == request.Id, cancellationToken);

        var hasPurchaseOrders = await _uow.Repository<PurchaseOrder>()
            .Query()
            .AnyAsync(x => x.CreatedByUserId == request.Id, cancellationToken);

        var hasMovements = await _uow.Repository<InventoryMovement>()
            .Query()
            .AnyAsync(x => x.UserId == request.Id, cancellationToken);

        if (hasInvoices || hasPurchaseOrders || hasMovements)
            return Result<string>.Failure(
                "Cannot delete this user because they have linked records (invoices/orders/movements). " +
                "Disable the account instead.", 400);

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
            return Result<string>.Failure(
                string.Join(", ", result.Errors.Select(e => e.Description)), 400);

        await _cache.RemoveByPatternAsync("users:*", cancellationToken);

        return Result<string>.Success("Deleted", "User deleted successfully");
    }
}