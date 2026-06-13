using MediatR;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Domain.Entities;

public class GetSalesReturnsQueryHandeler
: IRequestHandler<GetSalesReturnsQuery, List<SalesReturnDto>>
{
    private readonly IUnitOfWork _uow;


    public GetSalesReturnsQueryHandeler(IUnitOfWork uow)
    {
        _uow = uow;
    }
    public async Task<List<SalesReturnDto>> Handle(
GetSalesReturnsQuery request,
CancellationToken cancellationToken)
    {
        var query = _uow.Repository<SalesReturn>()
        .Query()
        .Include(x => x.Invoice)
        .Include(x => x.SalesReturnItems)
        .ThenInclude(i => i.Product)
        .Include(x => x.SalesReturnItems)
        .ThenInclude(i => i.Batch)
        .AsQueryable();


        if (request.InvoiceId.HasValue)
            query = query.Where(x => x.InvoiceId == request.InvoiceId);

        if (request.UserId.HasValue)
            query = query.Where(x => x.ReturnedByUserId == request.UserId);

        if (request.From.HasValue)
            query = query.Where(x => x.CreatedAt >= request.From);

        if (request.To.HasValue)
            query = query.Where(x => x.CreatedAt <= request.To);

        var data = await query.ToListAsync(cancellationToken);

        return data.Select(r => new SalesReturnDto
        {
            Id = r.Id,
            InvoiceId = r.InvoiceId,
            InvoiceNumber = r.Invoice.InvoiceNumber,
            ReturnedBy = r.ReturnedByUserId.ToString(),
            Reason = r.Reason,
            TotalAmount = r.TotalAmount,
            CreatedAt = r.CreatedAt,

            Items = r.SalesReturnItems.Select(i => new SalesReturnItemDto
            {
                ProductName = i.Product.Name,
                BatchNumber = i.Batch.BatchNumber,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                Total = i.Total
            }).ToList()
        }).ToList();

    }
}
