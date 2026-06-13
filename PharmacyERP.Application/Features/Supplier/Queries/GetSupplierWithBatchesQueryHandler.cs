using MediatR;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;

public class GetSupplierWithBatchesQueryHandler
    : IRequestHandler<GetSupplierWithBatchesQuery, Result<SupplierWithBatchesDto>>
{
    private readonly IUnitOfWork _uow;

    public GetSupplierWithBatchesQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<SupplierWithBatchesDto>> Handle(
        GetSupplierWithBatchesQuery request,
        CancellationToken cancellationToken)
    {
        var dto = await _uow.Repository<Supplier>()
            .Query()
            .AsNoTracking()
            .Where(x => x.Id == request.Id && !x.IsDeleted)
            .Select(s => new SupplierWithBatchesDto
            {
                Id = s.Id,
                Name = s.Name,

                Batches = s.ProductBatches.Select(b => new ProductBatchDto
                {
                    BatchNumber = b.BatchNumber,
                    Quantity = b.Quantity,
                    ExpiryDate = b.ExpiryDate
                }).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (dto == null)
            return Result<SupplierWithBatchesDto>
                .Failure("Supplier not found", 404);

        return Result<SupplierWithBatchesDto>
            .Success(dto, "Supplier retrieved successfully");
    }
}