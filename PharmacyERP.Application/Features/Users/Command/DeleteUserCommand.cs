using MediatR;
using PharmacyERP.Application.Common.Models;

public class DeleteUserCommand : IRequest<Result<string>>
{
    public int Id { get; set; }
}