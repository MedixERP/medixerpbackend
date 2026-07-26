using MediatR;
using PharmacyERP.Application.Common.Models;

public class DisableUserCommand : IRequest<Result<string>>
{
    public int Id { get; set; }
}