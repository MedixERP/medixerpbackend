using MediatR;
using PharmacyERP.Application.Common.Models;

public class ChangeUserRoleCommand : IRequest<Result<string>>
{
    public int Id { get; set; }
    public string NewRole { get; set; }
}