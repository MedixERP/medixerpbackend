using Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;

public class UpdateProductCommandHandler
    : IRequestHandler<UpdateProductCommand, Result<string>>
{
    private readonly IUnitOfWork _uow;
    private readonly IHttpContextAccessor _http;
    private readonly ICacheService _cache;

    public UpdateProductCommandHandler(
        IUnitOfWork uow,
        IHttpContextAccessor http,
        ICacheService cache)
    {
        _uow = uow;
        _http = http;
        _cache = cache;
    }

    public async Task<Result<string>> Handle(
        UpdateProductCommand request,
        CancellationToken cancellationToken)
    {
        var user = _http.HttpContext?.User;
        if (user == null || !user.Identity!.IsAuthenticated)
            return Result<string>.Failure("Unauthorized", 401);
        if (!user.IsInRole("Admin") && !user.IsInRole("Pharmacist"))
            return Result<string>.Failure("Forbidden", 403);

        var product = await _uow.Products.GetByIdAsync(request.Id);

        if (product == null || product.IsDeleted)
            return Result<string>.Failure("Product not found", 404);

        if (request.SalePrice <= request.PurchasePrice)
            return Result<string>.Failure("Invalid prices", 400);

        product.Name = request.Name.Trim();
        product.PurchasePrice = request.PurchasePrice;
        product.SalePrice = request.SalePrice;
        product.MinStockLevel = request.MinStockLevel;
        product.UpdatedAt = DateTime.UtcNow;

        _uow.Products.Update(product);
        await _uow.SaveChangesAsync(cancellationToken);

        await _cache.RemoveByPatternAsync("products:*", cancellationToken);

        return Result<string>.Success("Updated", "Product updated");
    }
}