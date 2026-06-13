using MediatR;
using Microsoft.AspNetCore.Http;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;

public class GetProductByIdQueryHandler
    : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IHttpContextAccessor _http;

    public GetProductByIdQueryHandler(IUnitOfWork uow, IHttpContextAccessor http)
    {
        _uow = uow;
        _http = http;
    }

    public async Task<Result<ProductDto>> Handle(
    GetProductByIdQuery request,
    CancellationToken cancellationToken)
    {
        var user = _http.HttpContext?.User;

        if (user == null || !user.Identity!.IsAuthenticated)
            return Result<ProductDto>.Failure("Unauthorized", 401);

        if (!user.IsInRole("Admin") &&
            !user.IsInRole("Pharmacist") &&
            !user.IsInRole("Cashier"))
        {
            return Result<ProductDto>.Failure("Forbidden", 403);
        }

        var product =
            await _uow.Products.GetByIdWithBatchesAsync(request.Id);

        if (product == null || product.IsDeleted)
            return Result<ProductDto>.Failure("Not found", 404);

        var stock = product.ProductBatches.Sum(b => b.Quantity);

        return Result<ProductDto>.Success(new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Barcode = product.Barcode,
            SalePrice = product.SalePrice,
            TotalStock = stock,
            IsLowStock = stock <= product.MinStockLevel,
            Batches = product.ProductBatches.Select(b => new ProductBatchDto
            {
                BatchNumber = b.BatchNumber,
                Quantity = b.Quantity,
                ExpiryDate = b.ExpiryDate
            }).ToList()
        });
    }
}