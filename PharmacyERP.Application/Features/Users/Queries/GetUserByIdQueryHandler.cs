using MediatR;
using Microsoft.AspNetCore.Identity;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;

public class GetUserByIdQueryHandler
    : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public GetUserByIdQueryHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<UserDto>> Handle(
        GetUserByIdQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id.ToString());

        if (user == null)
            return Result<UserDto>.Failure("User not found", 404);

        var roles = await _userManager.GetRolesAsync(user);

        var dto = new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            IsActive = user.IsActive,
            Roles = roles.ToList()
        };

        return Result<UserDto>.Success(dto, "User retrieved successfully");
    }
}