using MediatR;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;

public class UpdatePharmacyCompanyCommandHandler
    : IRequestHandler<UpdatePharmacyCompanyCommand, Result<string>>
{
    private readonly IUnitOfWork _uow;

    public UpdatePharmacyCompanyCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<string>> Handle(
        UpdatePharmacyCompanyCommand request,
        CancellationToken cancellationToken)
    {
        var company = await _uow.Repository<PharmacyCompany>()
            .GetByIdAsync(request.Id);

        if (company == null || company.IsDeleted)
            return Result<string>.Failure("Company not found", 404);

        company.Name = request.Name.Trim();
        company.Email = request.Email.Trim();
        company.Phone = request.Phone.Trim();
        company.Address = request.Address.Trim();
        company.UpdatedAt = DateTime.UtcNow;

        _uow.Repository<PharmacyCompany>().Update(company);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result<string>.Success(
            "Updated", "Company updated successfully");
    }
}