using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Application.Features.Auth.Queries;
using PharmacyERP.Domain.Entities;

public class LoginWithRefreshHandler
    : IRequestHandler<LoginWithRefreshCommand, Result<AuthResultDto>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtService _jwtService;
    private readonly IUnitOfWork _uow;

    public LoginWithRefreshHandler(
        UserManager<ApplicationUser> userManager,
        IJwtService jwtService,
        IUnitOfWork uow)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _uow = uow;
    }

    public async Task<Result<AuthResultDto>> Handle(
        LoginWithRefreshCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null || !user.IsActive)
            return Result<AuthResultDto>.Failure(
                "Invalid credentials or inactive account", 401);

        var passwordValid = await _userManager
            .CheckPasswordAsync(user, request.Password);

        if (!passwordValid)
            return Result<AuthResultDto>.Failure("Invalid credentials", 401);

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _jwtService.GenerateToken(user, roles);

        var refreshTokenValue = Convert.ToBase64String(
            System.Security.Cryptography.RandomNumberGenerator.GetBytes(64));

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshTokenValue,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.Repository<RefreshToken>().AddAsync(refreshToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result<AuthResultDto>.Success(
            new AuthResultDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenValue
            },
            "Login successful");
    }
}