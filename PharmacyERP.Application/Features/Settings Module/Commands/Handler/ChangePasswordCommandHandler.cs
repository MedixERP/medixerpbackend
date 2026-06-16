using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using PharmacyERP.Application.Common.Models;
using System.Security.Claims;
using PharmacyERP.Domain.Entities;

public class ChangePasswordCommandHandler
    : IRequestHandler<ChangePasswordCommand, Result<bool>>
{
    private readonly IHttpContextAccessor _http;
    private readonly UserManager<ApplicationUser> _userManager;

    public ChangePasswordCommandHandler(
        IHttpContextAccessor http,
        UserManager<ApplicationUser> userManager)
    {
        _http = http;
        _userManager = userManager;
    }

    public async Task<Result<bool>> Handle(
        ChangePasswordCommand request,
        CancellationToken cancellationToken)
    {
        var user = _http.HttpContext?.User;

        if (user == null || !user.Identity!.IsAuthenticated)
            return Result<bool>.Failure("Unauthorized", 401);

        var userIdValue = user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userIdValue))
            return Result<bool>.Failure("Invalid token", 401);

        var appUser = await _userManager.FindByIdAsync(userIdValue);

        if (appUser == null)
            return Result<bool>.Failure("User not found", 404);

        var result = await _userManager.ChangePasswordAsync(
            appUser,
            request.CurrentPassword,
            request.NewPassword);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result<bool>.Failure(errors, 400);
        }

        return Result<bool>.Success(true, "Password changed successfully");
    }
}