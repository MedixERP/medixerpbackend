using Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;

public class DeleteProductCommandHandler
    : IRequestHandler<DeleteProductCommand, Result<string>>
{
    private readonly IUnitOfWork _uow;
    private readonly IHttpContextAccessor _http;
    private readonly ICacheService _cache;

    public DeleteProductCommandHandler(
        IUnitOfWork uow,
        IHttpContextAccessor http,
        ICacheService cache)
    {
        _uow = uow;
        _http = http;
        _cache = cache;
    }

    public async Task<Result<string>> Handle(
     DeleteProductCommand request,
     CancellationToken cancellationToken)
    {
        var user = _http.HttpContext?.User;
        if (user == null || !user.Identity!.IsAuthenticated)
            return Result<string>.Failure("Unauthorized", 401);
        if (!user.IsInRole("Admin"))
            return Result<string>.Failure("Forbidden", 403);

        var product =
            await _uow.Products.GetByIdWithBatchesAsync(request.Id);

        if (product == null || product.IsDeleted)
            return Result<string>.Failure("Product not found", 404);

        var hasStock = product.ProductBatches.Any(b => b.Quantity > 0);
        if (hasStock)
            return Result<string>.Failure("Cannot delete product with stock", 400);

        product.IsDeleted = true;
        product.UpdatedAt = DateTime.UtcNow;

        _uow.Products.Update(product);
        await _uow.SaveChangesAsync(cancellationToken);

        await _cache.RemoveByPatternAsync("products:*", cancellationToken);

        return Result<string>.Success("Deleted", "Product deleted");
    }
}