using MediatR;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;

public class LoginHandler : IRequestHandler<LoginCommand, Result<string>>
{
    private readonly IAuthService _authService;

    public LoginHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<Result<string>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var token = await _authService.LoginAsync(
                request.Email,
                request.Password);

            return Result<string>.Success(token, "Login successful");
        }
        catch (Exception ex)
        {
            return Result<string>.Failure(ex.Message, 401);
        }
    }
}