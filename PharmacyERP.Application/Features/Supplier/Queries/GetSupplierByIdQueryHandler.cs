using MediatR;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;

public class GetSupplierByIdQueryHandler
    : IRequestHandler<GetSupplierByIdQuery, Result<SupplierDto>>
{
    private readonly IUnitOfWork _uow;

    public GetSupplierByIdQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<SupplierDto>> Handle(
        GetSupplierByIdQuery request,
        CancellationToken cancellationToken)
    {
        var supplier = await _uow.Repository<Supplier>()
            .GetByIdAsync(request.Id);

        if (supplier == null || supplier.IsDeleted)
            return Result<SupplierDto>.Failure("Supplier not found", 404);

        var dto = new SupplierDto
        {
            Id = supplier.Id,
            Name = supplier.Name,
            Phone = supplier.Phone,
            Address = supplier.Address
        };

        return Result<SupplierDto>.Success(dto);
    }
}