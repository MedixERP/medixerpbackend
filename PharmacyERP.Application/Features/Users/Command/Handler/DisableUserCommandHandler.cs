using Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;
using System.Security.Claims;

public class DisableUserCommandHandler
    : IRequestHandler<DisableUserCommand, Result<string>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpContextAccessor _http;
    private readonly ICacheService _cache;

    public DisableUserCommandHandler(
        UserManager<ApplicationUser> userManager,
        IHttpContextAccessor http,
        ICacheService cache)
    {
        _userManager = userManager;
        _http = http;
        _cache = cache;
    }

    public async Task<Result<string>> Handle(
        DisableUserCommand request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _http.HttpContext?.User?
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (currentUserId != null && int.Parse(currentUserId) == request.Id)
            return Result<string>.Failure("You cannot disable your own account", 400);

        var user = await _userManager.FindByIdAsync(request.Id.ToString());
        if (user == null)
            return Result<string>.Failure("User not found", 404);

        if (!user.IsActive)
            return Result<string>.Failure("User is already disabled", 400);

        user.IsActive = false;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return Result<string>.Failure(
                string.Join(", ", result.Errors.Select(e => e.Description)), 400);

        await _cache.RemoveByPatternAsync("users:*", cancellationToken);

        return Result<string>.Success("Disabled", "User disabled successfully");
    }
}