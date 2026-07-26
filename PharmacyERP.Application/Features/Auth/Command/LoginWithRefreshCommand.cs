using MediatR;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Application.Features.Auth.Queries;

public class LoginWithRefreshCommand : IRequest<Result<AuthResultDto>>
{
    public string Email { get; set; }
    public string Password { get; set; }
}