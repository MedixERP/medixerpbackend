using MediatR;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Application.Features.Auth.Queries;

public class RefreshTokenCommand : IRequest<Result<AuthResultDto>>
{
    public string RefreshToken { get; set; }
}