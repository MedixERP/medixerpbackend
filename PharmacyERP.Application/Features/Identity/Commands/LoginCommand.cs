using MediatR;
using PharmacyERP.Application.Common.Models;

public record LoginCommand(
    string Email,
    string Password
) : IRequest<Result<string>>;