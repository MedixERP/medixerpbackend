using MediatR;
using Microsoft.AspNetCore.Http;
using PharmacyERP.Application.Common.Models;
using System.Security.Claims;

public class UploadAvatarCommandHandler
    : IRequestHandler<UploadAvatarCommand, Result<string>>
{
    private readonly IHttpContextAccessor _http;
    private readonly IUnitOfWork _uow;
    private readonly IFileService _fileService;

    public UploadAvatarCommandHandler(
        IHttpContextAccessor http,
        IUnitOfWork uow,
        IFileService fileService)
    {
        _http = http;
        _uow = uow;
        _fileService = fileService;
    }

    public async Task<Result<string>> Handle(
        UploadAvatarCommand request,
        CancellationToken cancellationToken)
    {
        var user = _http.HttpContext?.User;

        if (user == null || !user.Identity!.IsAuthenticated)
            return Result<string>.Failure("Unauthorized", 401);

        var userIdValue = user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userIdValue))
            return Result<string>.Failure("Invalid token", 401);

        var userId = int.Parse(userIdValue);

        var settings = await _uow.UserSettings
            .GetByUserIdAsync(userId);

        if (settings == null)
            return Result<string>.Failure("Settings not found", 404);

        var url = await _fileService.SaveAvatarAsync(
            request.File,
            settings.ProfileImageUrl
        );

        settings.ProfileImageUrl = url;

        await _uow.SaveChangesAsync(cancellationToken);

        return Result<string>.Success(url, "Avatar uploaded successfully");
    }
}