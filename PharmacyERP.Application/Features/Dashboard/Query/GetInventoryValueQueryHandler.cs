using MediatR;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Domain.Entities;

public class GetInventoryValueQueryHandler
    : IRequestHandler<GetInventoryValueQuery, decimal>
{
    private readonly IUnitOfWork _uow;

    public GetInventoryValueQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<decimal> Handle(
        GetInventoryValueQuery request,
        CancellationToken cancellationToken)
    {
        var batches = await _uow.Repository<ProductBatch>()
            .Query()
            .Include(x => x.Product)
            .ToListAsync(cancellationToken);

        decimal totalValue = batches.Sum(x =>
            x.Quantity * x.Product.PurchasePrice);

        return totalValue;
    }
}