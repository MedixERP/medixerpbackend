using MediatR;
using Microsoft.AspNetCore.Http;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;

public class GetProductBatchesQueryHandler
    : IRequestHandler<GetProductBatchesQuery, Result<List<ProductBatchsDto>>>
{
    private readonly IUnitOfWork _uow;
    private readonly IHttpContextAccessor _http;

    public GetProductBatchesQueryHandler(IUnitOfWork uow, IHttpContextAccessor http)
    {
        _uow = uow;
        _http = http;
    }

    public async Task<Result<List<ProductBatchsDto>>> Handle(
        GetProductBatchesQuery request,
        CancellationToken cancellationToken)
    {
        var user = _http.HttpContext?.User;

        if (user == null || !user.Identity!.IsAuthenticated)
            return Result<List<ProductBatchsDto>>.Failure("Unauthorized", 401);

        var batches = await _uow.ProductBatches.GetByProductIdAsync(request.ProductId);

        var result = batches.Select(x => new ProductBatchsDto
        {
            Id = x.Id,
            BatchNumber = x.BatchNumber,
            Quantity = x.Quantity,
            ExpiryDate = x.ExpiryDate,
            PurchasePrice = x.PurchasePrice,
            ProductId = x.ProductId,
            ProductName = x.Product.Name,
            IsExpired = x.ExpiryDate < DateTime.UtcNow
        }).ToList();

        return Result<List<ProductBatchsDto>>.Success(result);
    }
}