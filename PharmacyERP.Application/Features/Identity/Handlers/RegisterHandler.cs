using MediatR;
using Microsoft.AspNetCore.Http;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;

public class RegisterHandler : IRequestHandler<RegisterCommand, Result<string>>
{
    private readonly IAuthService _authService;
    private readonly IHttpContextAccessor _http;

    public RegisterHandler(
        IAuthService authService,
        IHttpContextAccessor http)
    {
        _authService = authService;
        _http = http;
    }

    public async Task<Result<string>> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        var user = _http.HttpContext?.User;

        if (user == null || !user.Identity!.IsAuthenticated)
            return Result<string>.Failure("Unauthorized", 401);

        if (!user.IsInRole("Admin"))
            return Result<string>.Failure("Only Admin can create users", 403);

        try
        {
            var newUser = await _authService.RegisterAsync(
                request.Email,
                request.Password,
                request.FullName,
                request.Role);

            return Result<string>.Success(
                newUser.Id.ToString(),
                "User Created Successfully");
        }
        catch (Exception ex)
        {
            return Result<string>.Failure(ex.Message, 400);
        }
    }
}