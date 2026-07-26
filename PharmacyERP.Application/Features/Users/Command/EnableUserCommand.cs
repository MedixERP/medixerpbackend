using MediatR;
using PharmacyERP.Application.Common.Models;

public class EnableUserCommand : IRequest<Result<string>>
{
    public int Id { get; set; }
}