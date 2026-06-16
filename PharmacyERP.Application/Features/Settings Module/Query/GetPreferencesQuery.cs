using MediatR;
using PharmacyERP.Application.Common.Models;

public class GetPreferencesQuery : IRequest<Result<PreferencesDto>>
{
}