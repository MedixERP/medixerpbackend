using MediatR;
using PharmacyERP.Application.Common.Models;

public class DisablePharmacyCompanyCommand : IRequest<Result<string>>
{
    public int Id { get; set; }
}