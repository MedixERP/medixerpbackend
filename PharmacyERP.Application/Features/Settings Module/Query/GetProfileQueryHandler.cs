using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;
using System.Security.Claims;

public class GetProfileQueryHandler
    : IRequestHandler<GetProfileQuery, Result<Profile2Dto>>
{
    private readonly IHttpContextAccessor _http;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetProfileQueryHandler(
        IHttpContextAccessor http,
        UserManager<ApplicationUser> userManager)
    {
        _http = http;
        _userManager = userManager;
    }

    public async Task<Result<Profile2Dto>> Handle(
        GetProfileQuery request,
        CancellationToken cancellationToken)
    {
        var user = _http.HttpContext?.User;

        if (user == null || !user.Identity!.IsAuthenticated)
            return Result<Profile2Dto>.Failure("Unauthorized", 401);

        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Result<Profile2Dto>.Failure("Invalid token", 401);

        var appUser = await _userManager.FindByIdAsync(userId);

        if (appUser == null)
            return Result<Profile2Dto>.Failure("User not found", 404);

        var roles = await _userManager.GetRolesAsync(appUser);

        var dto = new Profile2Dto
        {
            FullName = appUser.FullName,
            Email = appUser.Email,
            Phone = appUser.PhoneNumber,
            Role = roles.FirstOrDefault() ?? ""
        };

        return Result<Profile2Dto>.Success(dto);
    }
}