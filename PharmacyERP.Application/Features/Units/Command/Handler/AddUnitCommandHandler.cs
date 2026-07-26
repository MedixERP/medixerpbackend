using MediatR;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;

public class AddUnitCommandHandler
    : IRequestHandler<AddUnitCommand, Result<int>>
{
    private readonly IUnitOfWork _uow;

    public AddUnitCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<int>> Handle(
        AddUnitCommand request,
        CancellationToken cancellationToken)
    {
        var unit = new PharmacyERP.Domain.Entities.Unit
        {
            Name = request.Name.Trim(),
            Symbol = request.Symbol.Trim(),
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await _uow.Repository<PharmacyERP.Domain.Entities.Unit>().AddAsync(unit);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(unit.Id, "Unit created successfully");
    }
}