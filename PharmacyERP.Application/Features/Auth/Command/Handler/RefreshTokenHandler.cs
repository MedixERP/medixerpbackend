using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Application.Features.Auth.Queries;
using PharmacyERP.Domain.Entities;

public class RefreshTokenHandler
    : IRequestHandler<RefreshTokenCommand, Result<AuthResultDto>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtService _jwtService;
    private readonly IUnitOfWork _uow;

    public RefreshTokenHandler(
        UserManager<ApplicationUser> userManager,
        IJwtService jwtService,
        IUnitOfWork uow)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _uow = uow;
    }

    public async Task<Result<AuthResultDto>> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        var existing = await _uow.Repository<RefreshToken>()
            .Query()
            .FirstOrDefaultAsync(
                x => x.Token == request.RefreshToken,
                cancellationToken);

        if (existing == null || existing.IsRevoked ||
            existing.ExpiresAt < DateTime.UtcNow)
            return Result<AuthResultDto>.Failure(
                "Invalid or expired refresh token", 401);

        var user = await _userManager
            .FindByIdAsync(existing.UserId.ToString());

        if (user == null || !user.IsActive)
            return Result<AuthResultDto>.Failure(
                "User not found or inactive", 401);

        existing.IsRevoked = true;
        _uow.Repository<RefreshToken>().Update(existing);

        var roles = await _userManager.GetRolesAsync(user);
        var newAccessToken = _jwtService.GenerateToken(user, roles);

        var newRefreshValue = Convert.ToBase64String(
            System.Security.Cryptography.RandomNumberGenerator.GetBytes(64));

        var newRefreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = newRefreshValue,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.Repository<RefreshToken>().AddAsync(newRefreshToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result<AuthResultDto>.Success(
            new AuthResultDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshValue
            },
            "Token refreshed successfully");
    }
}