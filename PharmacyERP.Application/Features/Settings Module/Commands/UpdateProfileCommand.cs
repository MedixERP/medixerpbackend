using MediatR;
using PharmacyERP.Application.Common.Models;

public class UpdateProfileCommand : IRequest<Result<bool>>
{
    public string FullName { get; set; }
    public string Phone { get; set; }
}