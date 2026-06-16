using MediatR;
using PharmacyERP.Application.Common.Models;

public class ChangePasswordCommand : IRequest<Result<bool>>
{
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
}