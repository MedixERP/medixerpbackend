using MediatR;
using PharmacyERP.Application.Common.Models;

public class GetUserByIdQuery : IRequest<Result<UserDto>>
{
    public int Id { get; set; }
}