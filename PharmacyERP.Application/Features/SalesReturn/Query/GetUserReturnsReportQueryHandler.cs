using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Domain.Entities;
using System.Security.Claims;

public class GetUserReturnsReportQueryHandler
: IRequestHandler<GetUserReturnsReportQuery, List<SalesReturnDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IHttpContextAccessor _http;


public GetUserReturnsReportQueryHandler(
    IUnitOfWork uow,
    IHttpContextAccessor http)
    {
        _uow = uow;
        _http = http;
    }

    public async Task<List<SalesReturnDto>> Handle(
        GetUserReturnsReportQuery request,
        CancellationToken cancellationToken)
    {
        var userId = int.Parse(
            _http.HttpContext!.User
                .FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var data = await _uow.Repository<SalesReturn>()
            .Query()
            .AsNoTracking()
            .Where(x => x.ReturnedByUserId == userId)
            .Include(x => x.Invoice)
            .Include(x => x.ReturnedByUser)
            .Include(x => x.SalesReturnItems)
                .ThenInclude(i => i.Product)
            .Include(x => x.SalesReturnItems)
                .ThenInclude(i => i.Batch)
            .ToListAsync(cancellationToken);

        return data.Select(x => new SalesReturnDto
        {
            Id = x.Id,
            InvoiceId = x.InvoiceId,
            InvoiceNumber = x.Invoice.InvoiceNumber,

            ReturnedBy = x.ReturnedByUser != null
                ? x.ReturnedByUser.FullName
                : "Unknown",

            Reason = x.Reason,
            TotalAmount = x.TotalAmount,
            CreatedAt = x.CreatedAt,

            Items = x.SalesReturnItems?.Select(i => new SalesReturnItemDto
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
