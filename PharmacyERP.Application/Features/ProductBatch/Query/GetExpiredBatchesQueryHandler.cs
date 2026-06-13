using MediatR;
using Microsoft.AspNetCore.Http;
using PharmacyERP.Application.Common.Interfaces;

public class GetExpiredBatchesQueryHandler
    : IRequestHandler<
        GetExpiredBatchesQuery,
        List<ProductBatchsDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IHttpContextAccessor _http;

    public GetExpiredBatchesQueryHandler(
        IUnitOfWork uow,
        IHttpContextAccessor http)
    {
        _uow = uow;
        _http = http;
    }

    public async Task<List<ProductBatchsDto>> Handle(
        GetExpiredBatchesQuery request,
        CancellationToken cancellationToken)
    {
        var user = _http.HttpContext?.User;

        if (user == null || !user.Identity!.IsAuthenticated)
            throw new Exception("Unauthorized");

        var batches = await _uow.ProductBatches
            .GetExpiredBatchesAsync();

        return batches.Select(x => new ProductBatchsDto
        {
            Id = x.Id,
            BatchNumber = x.BatchNumber,
            Quantity = x.Quantity,
            ExpiryDate = x.ExpiryDate,
            PurchasePrice = x.PurchasePrice,
            ProductId = x.ProductId,
            ProductName = x.Product.Name,
            IsExpired = true
        }).ToList();
    }
}