using MediatR;
using Microsoft.AspNetCore.Http;
using PharmacyERP.Application.Common.Models;
using System.Security.Claims;

public class AddToCartCommandHandler
    : IRequestHandler<AddToCartCommand, Result<List<PosCartItemDto>>>
{
    private readonly IMediator _mediator;
    private readonly ICartService _cart;
    private readonly IHttpContextAccessor _http;

    public AddToCartCommandHandler(
        IMediator mediator,
        ICartService cart,
        IHttpContextAccessor http)
    {
        _mediator = mediator;
        _cart = cart;
        _http = http;
    }

    public async Task<Result<List<PosCartItemDto>>> Handle(
       AddToCartCommand request,
       CancellationToken cancellationToken)
    {
        var httpContext = _http.HttpContext;

        if (httpContext == null)
            return Result<List<PosCartItemDto>>.Failure("HttpContext is null", 500);

        var user = httpContext.User;

        if (user?.Identity?.IsAuthenticated != true)
            return Result<List<PosCartItemDto>>.Failure("Unauthorized", 401);

        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(userId))
            return Result<List<PosCartItemDto>>.Failure("UserId not found", 400);

        var scanResult = await _mediator.Send(new ScanBarcodeQuery
        {
            Barcode = request.Barcode
        }, cancellationToken);

        if (!scanResult.IsSuccess)
            return Result<List<PosCartItemDto>>.Failure(
                scanResult.Message,
                scanResult.StatusCode);

        var scan = scanResult.Data;

        var item = new PosCartItemDto
        {
            ProductId = scan.ProductId,
            BatchId = scan.BatchId,
            ProductName = scan.ProductName,
            UnitPrice = scan.Price,
            Quantity = request.Quantity,
           
        };

        _cart.AddItem(userId, item);

        return Result<List<PosCartItemDto>>.Success(
            _cart.GetCart(userId),
            "Item added successfully");
    }
}