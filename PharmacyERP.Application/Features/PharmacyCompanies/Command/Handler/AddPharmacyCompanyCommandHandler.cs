using MediatR;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;

public class AddPharmacyCompanyCommandHandler
    : IRequestHandler<AddPharmacyCompanyCommand, Result<int>>
{
    private readonly IUnitOfWork _uow;

    public AddPharmacyCompanyCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<int>> Handle(
        AddPharmacyCompanyCommand request,
        CancellationToken cancellationToken)
    {
        var exists = await _uow.Repository<PharmacyCompany>()
            .Query()
            .AnyAsync(x => x.Name == request.Name.Trim()
                        && !x.IsDeleted,
                cancellationToken);

        if (exists)
            return Result<int>.Failure(
                "Company with this name already exists", 400);

        var company = new PharmacyCompany
        {
            Name = request.Name.Trim(),
            Email = request.Email.Trim(),
            Phone = request.Phone.Trim(),
            Address = request.Address.Trim(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.Repository<PharmacyCompany>().AddAsync(company);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(
            company.Id, "Company added successfully");
    }
}