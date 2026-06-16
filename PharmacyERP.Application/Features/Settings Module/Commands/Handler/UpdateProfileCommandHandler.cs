using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using PharmacyERP.Application.Common.Models;
using System.Security.Claims;
using PharmacyERP.Domain.Entities;

public class UpdateProfileCommandHandler
    : IRequestHandler<UpdateProfileCommand, Result<bool>>
{
    private readonly IHttpContextAccessor _http;
    private readonly UserManager<ApplicationUser> _userManager;

    public UpdateProfileCommandHandler(
        IHttpContextAccessor http,
        UserManager<ApplicationUser> userManager)
    {
        _http = http;
        _userManager = userManager;
    }

    public async Task<Result<bool>> Handle(
        UpdateProfileCommand request,
        CancellationToken cancellationToken)
    {
        var user = _http.HttpContext?.User;

        if (user == null || !user.Identity!.IsAuthenticated)
            return Result<bool>.Failure("Unauthorized", 401);

        var userIdValue = user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userIdValue))
            return Result<bool>.Failure("Invalid token", 401);

        var userId = int.Parse(userIdValue);

        var appUser = await _userManager.FindByIdAsync(userId.ToString());

        if (appUser == null)
            return Result<bool>.Failure("User not found", 404);

        appUser.FullName = request.FullName.Trim();
        appUser.PhoneNumber = request.Phone.Trim();

        var result = await _userManager.UpdateAsync(appUser);

        if (!result.Succeeded)
            return Result<bool>.Failure("Update failed");

        return Result<bool>.Success(true, "Profile updated successfully");
    }
}