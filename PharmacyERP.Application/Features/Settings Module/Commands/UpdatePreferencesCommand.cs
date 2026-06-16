using MediatR;
using PharmacyERP.Application.Common.Models;

public class UpdatePreferencesCommand : IRequest<Result<bool>>
{
    public string Language { get; set; }
    public string Theme { get; set; }
}