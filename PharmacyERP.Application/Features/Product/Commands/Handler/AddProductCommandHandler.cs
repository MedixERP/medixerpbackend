using Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using PharmacyERP.Application.Common.Interfaces;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Domain.Entities;

public class AddProductCommandHandler
    : IRequestHandler<AddProductCommand, Result<int>>
{
    private readonly IUnitOfWork _uow;
    private readonly IHttpContextAccessor _http;
    private readonly IBarcodeService _barcodeService;
    private readonly ICacheService _cache;

    public AddProductCommandHandler(
        IUnitOfWork uow,
        IHttpContextAccessor http,
        IBarcodeService barcodeService,
        ICacheService cache)
    {
        _uow = uow;
        _http = http;
        _barcodeService = barcodeService;
        _cache = cache;
    }

    public async Task<Result<int>> Handle(
        AddProductCommand request,
        CancellationToken cancellationToken)
    {
        var user = _http.HttpContext?.User;
        if (user == null || !user.Identity!.IsAuthenticated)
            return Result<int>.Failure("Unauthorized", 401);
        if (!user.IsInRole("Admin") && !user.IsInRole("Pharmacist"))
            return Result<int>.Failure("Forbidden", 403);

        var barcode = _barcodeService.GenerateBarcodeValue();

        var product = new Product
        {
            Name = request.Name.Trim(),
            ScientificName = request.ScientificName?.Trim(),
            CategoryId = request.CategoryId,
            PurchasePrice = request.PurchasePrice,
            SalePrice = request.SalePrice,
            MinStockLevel = request.MinStockLevel,
            Barcode = barcode,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.Products.AddAsync(product);
        await _uow.SaveChangesAsync(cancellationToken);

        product.BarcodeImage = _barcodeService.GenerateBarcode(barcode);
        product.QrCodeImage = _barcodeService.GenerateQrCode(
            $"ProductId={product.Id}&Barcode={barcode}");

        await _uow.SaveChangesAsync(cancellationToken);

        await _cache.RemoveByPatternAsync("products:*", cancellationToken);

        return Result<int>.Success(product.Id, "Product created");
    }
}