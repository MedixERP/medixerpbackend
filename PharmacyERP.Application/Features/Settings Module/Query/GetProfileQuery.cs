using MediatR;
using PharmacyERP.Application.Common.Models;

public class GetProfileQuery : IRequest<Result<Profile2Dto>>
{
}