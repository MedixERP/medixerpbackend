using Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;

public class ChangeUserRoleCommandHandler
    : IRequestHandler<ChangeUserRoleCommand, Result<string>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private readonly ICacheService _cache;

    public ChangeUserRoleCommandHandler(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<int>> roleManager,
        ICacheService cache)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _cache = cache;
    }

    public async Task<Result<string>> Handle(
        ChangeUserRoleCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id.ToString());
        if (user == null)
            return Result<string>.Failure("User not found", 404);

        var roleExists = await _roleManager.RoleExistsAsync(request.NewRole);
        if (!roleExists)
            return Result<string>.Failure("Role does not exist", 400);

        var currentRoles = await _userManager.GetRolesAsync(user);
        if (currentRoles.Contains(request.NewRole))
            return Result<string>.Failure("User already has this role", 400);

        var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
        if (!removeResult.Succeeded)
            return Result<string>.Failure(
                string.Join(", ", removeResult.Errors.Select(e => e.Description)), 400);

        var addResult = await _userManager.AddToRoleAsync(user, request.NewRole);
        if (!addResult.Succeeded)
            return Result<string>.Failure(
                string.Join(", ", addResult.Errors.Select(e => e.Description)), 400);

        await _cache.RemoveByPatternAsync("users:*", cancellationToken);

        return Result<string>.Success(
            "Updated", $"User role changed to {request.NewRole} successfully");
    }
}