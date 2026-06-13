using MediatR;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;

public class GetProductBatchByIdQueryHandler
    : IRequestHandler<GetProductBatchByIdQuery, Result<ProductBatchsDto>>
{
    private readonly IUnitOfWork _uow;

    public GetProductBatchByIdQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<ProductBatchsDto>> Handle(
        GetProductBatchByIdQuery request,
        CancellationToken cancellationToken)
    {
        var batch = await _uow.Repository<ProductBatch>()
            .Query()
            .AsNoTracking()
            .Include(x => x.Product)
            .FirstOrDefaultAsync(
                x => x.Id == request.Id,
                cancellationToken);

        if (batch == null)
        {
            return Result<ProductBatchsDto>.Failure(
                "Batch not found",
                404);
        }

        var dto = new ProductBatchsDto
        {
            Id = batch.Id,
            BatchNumber = batch.BatchNumber,
            Quantity = batch.Quantity,
            ExpiryDate = batch.ExpiryDate,
            PurchasePrice = batch.PurchasePrice,

            IsExpired =
                batch.ExpiryDate.Date < DateTime.UtcNow.Date,

            ProductId = batch.ProductId,
            ProductName = batch.Product.Name
        };

        return Result<ProductBatchsDto>.Success(
            dto,
            "Batch retrieved successfully");
    }
}