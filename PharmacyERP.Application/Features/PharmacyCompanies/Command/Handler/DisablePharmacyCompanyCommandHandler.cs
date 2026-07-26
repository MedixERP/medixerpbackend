using MediatR;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;

public class DisablePharmacyCompanyCommandHandler
    : IRequestHandler<DisablePharmacyCompanyCommand, Result<string>>
{
    private readonly IUnitOfWork _uow;

    public DisablePharmacyCompanyCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<string>> Handle(
        DisablePharmacyCompanyCommand request,
        CancellationToken cancellationToken)
    {
        var company = await _uow.Repository<PharmacyCompany>()
            .GetByIdAsync(request.Id);

        if (company == null || company.IsDeleted)
            return Result<string>.Failure("Company not found", 404);

        if (!company.IsActive)
            return Result<string>.Failure(
                "Company is already disabled", 400);

        company.IsActive = false;
        company.UpdatedAt = DateTime.UtcNow;

        _uow.Repository<PharmacyCompany>().Update(company);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result<string>.Success(
            "Disabled", "Company disabled successfully");
    }
}