using MediatR;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;

public class GetSalesReturnByIdQueryHandler
    : IRequestHandler<GetSalesReturnByIdQuery, Result<SalesReturnDto>>
{
    private readonly IUnitOfWork _uow;

    public GetSalesReturnByIdQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<SalesReturnDto>> Handle(GetSalesReturnByIdQuery request, CancellationToken cancellationToken)
    {
        var r = await _uow.Repository<SalesReturn>()
            .Query()
            .Include(x => x.Invoice)
            .Include(x => x.SalesReturnItems)
                .ThenInclude(i => i.Product)
            .Include(x => x.SalesReturnItems)
                .ThenInclude(i => i.Batch)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (r == null)
            return Result<SalesReturnDto>.Failure("Return not found", 404);

        return Result<SalesReturnDto>.Success(new SalesReturnDto
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
        });
    }
}