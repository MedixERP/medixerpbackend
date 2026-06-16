using MediatR;
using PharmacyERP.Application.Common.Models;

public class RegisterCommand : IRequest<Result<string>>
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string FullName { get; set; }
    public string Phone { get; set; }
    public string Role { get; set; }
}