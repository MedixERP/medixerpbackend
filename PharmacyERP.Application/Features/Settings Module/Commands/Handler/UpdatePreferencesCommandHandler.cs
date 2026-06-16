using MediatR;
using Microsoft.AspNetCore.Http;
using PharmacyERP.Application.Common.Models;
using System.Security.Claims;

public class UpdatePreferencesCommandHandler
    : IRequestHandler<UpdatePreferencesCommand, Result<bool>>
{
    private readonly IUnitOfWork _uow;
    private readonly IHttpContextAccessor _http;

    public UpdatePreferencesCommandHandler(
        IUnitOfWork uow,
        IHttpContextAccessor http)
    {
        _uow = uow;
        _http = http;
    }

    public async Task<Result<bool>> Handle(
        UpdatePreferencesCommand request,
        CancellationToken cancellationToken)
    {
        var user = _http.HttpContext?.User;

        if (user == null || !user.Identity!.IsAuthenticated)
            return Result<bool>.Failure("Unauthorized", 401);

        var userIdValue = user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userIdValue))
            return Result<bool>.Failure("Invalid token", 401);

        var userId = int.Parse(userIdValue);

        var settings = await _uow.UserSettings
            .GetByUserIdAsync(userId);

        if (settings == null)
            return Result<bool>.Failure("Settings not found", 404);

        settings.Language = request.Language.Trim();
        settings.Theme = request.Theme.Trim();

        await _uow.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "Preferences updated");
    }
}