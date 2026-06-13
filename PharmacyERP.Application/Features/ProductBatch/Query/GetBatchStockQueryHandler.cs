using MediatR;
using Microsoft.AspNetCore.Http;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;

public class GetBatchStockQueryHandler
    : IRequestHandler<GetBatchStockQuery, Result<int>>
{
    private readonly IUnitOfWork _uow;
    private readonly IHttpContextAccessor _http;

    public GetBatchStockQueryHandler(
        IUnitOfWork uow,
        IHttpContextAccessor http)
    {
        _uow = uow;
        _http = http;
    }

    public async Task<Result<int>> Handle(
        GetBatchStockQuery request,
        CancellationToken cancellationToken)
    {
        
        var user = _http.HttpContext?.User;

        if (user == null || !user.Identity!.IsAuthenticated)
        {
            return Result<int>.Failure("Unauthorized", 401);
        }

        if (request.ProductId <= 0)
        {
            return Result<int>.Failure("Invalid ProductId", 400);
        }

       
        var stock = await _uow.ProductBatches
            .GetTotalStockAsync(request.ProductId);

        return Result<int>.Success(stock, "Stock retrieved successfully");
    }
}