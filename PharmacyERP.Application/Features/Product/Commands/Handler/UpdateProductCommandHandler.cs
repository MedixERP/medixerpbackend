using MediatR;
using Microsoft.AspNetCore.Http;
using PharmacyERP.Application.Common.Models;

public class UpdateProductCommandHandler
    : IRequestHandler<UpdateProductCommand, Result<string>>
{
    private readonly IUnitOfWork _uow;
    private readonly IHttpContextAccessor _http;

    public UpdateProductCommandHandler(IUnitOfWork uow, IHttpContextAccessor http)
    {
        _uow = uow;
        _http = http;
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

        return Result<string>.Success("Updated", "Product updated");
    }
}