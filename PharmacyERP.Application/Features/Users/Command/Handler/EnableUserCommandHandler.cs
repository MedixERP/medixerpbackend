using Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;

public class EnableUserCommandHandler
    : IRequestHandler<EnableUserCommand, Result<string>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICacheService _cache;

    public EnableUserCommandHandler(
        UserManager<ApplicationUser> userManager,
        ICacheService cache)
    {
        _userManager = userManager;
        _cache = cache;
    }

    public async Task<Result<string>> Handle(
        EnableUserCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id.ToString());
        if (user == null)
            return Result<string>.Failure("User not found", 404);

        if (user.IsActive)
            return Result<string>.Failure("User is already active", 400);

        user.IsActive = true;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return Result<string>.Failure(
                string.Join(", ", result.Errors.Select(e => e.Description)), 400);

        await _cache.RemoveByPatternAsync("users:*", cancellationToken);

        return Result<string>.Success("Enabled", "User enabled successfully");
    }
}