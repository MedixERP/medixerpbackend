using MediatR;
using Microsoft.AspNetCore.Http;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;
using System.Security.Claims;

public class GetPreferencesQueryHandler
    : IRequestHandler<GetPreferencesQuery, Result<PreferencesDto>>
{
    private readonly IHttpContextAccessor _http;
    private readonly IUnitOfWork _uow;

    public GetPreferencesQueryHandler(
        IHttpContextAccessor http,
        IUnitOfWork uow)
    {
        _http = http;
        _uow = uow;
    }

    public async Task<Result<PreferencesDto>> Handle(
        GetPreferencesQuery request,
        CancellationToken cancellationToken)
    {
        var user = _http.HttpContext?.User;

        if (user == null || !user.Identity!.IsAuthenticated)
            return Result<PreferencesDto>.Failure("Unauthorized", 401);

        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Result<PreferencesDto>.Failure("Invalid token", 401);

      
        var settings = await _uow.UserSettings
            .GetByUserIdAsync(int.Parse(userId));

        if (settings == null)
        {
            settings = new UserSettings
            {
                UserId = int.Parse(userId),
                Language = "en",
                Theme = "light",
                CreatedAt = DateTime.UtcNow
            };

            await _uow.UserSettings.AddAsync(settings);
            await _uow.SaveChangesAsync(cancellationToken);
        }

        var dto = new PreferencesDto
        {
            Language = settings.Language,
            Theme = settings.Theme,
            AvatarUrl = settings.ProfileImageUrl
        };

        return Result<PreferencesDto>.Success(dto);
    }
}