using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmacyERP.Domain.Entities;

[ApiController]
[Route("api/barcode")]
public class BarcodeController : ControllerBase
{
    private readonly IBarcodeService _barcodeService;
    private readonly IUnitOfWork _uow;
    private readonly IMediator _mediator;

    public BarcodeController(IBarcodeService barcodeService, IUnitOfWork uow, IMediator mediator)
    {
        _barcodeService = barcodeService;
        _uow = uow;
        _mediator = mediator;
    }

  
    [Authorize(Roles = "Admin,Pharmacist")]
    [HttpGet("generate")]
    public IActionResult Generate()
    {
        var code = _barcodeService.GenerateBarcodeValue();
        var image = _barcodeService.GenerateBarcode(code);

        return Ok(new
        {
            Barcode = code,
            ImageBase64 = Convert.ToBase64String(image)
        });
    }

    [Authorize(Roles = "Admin,Pharmacist,Cashier")]
    [HttpGet("product/{id}/barcode")]
    public async Task<IActionResult> GetProductBarcode(int id)
    {
        var product = await _uow.Products.GetByIdAsync(id);

        if (product == null)
            return NotFound();

        return Ok(new
        {
            product.Id,
            product.Name,
            product.Barcode,
            BarcodeImage = Convert.ToBase64String(product.BarcodeImage)
        });
    }

 
    [Authorize(Roles = "Admin,Pharmacist")]
    [HttpPost("product/{id}/regenerate")]
    public async Task<IActionResult> RegenerateBarcode(int id)
    {
        var product = await _uow.Products.GetByIdAsync(id);

        if (product == null)
            return NotFound();

        var newCode = _barcodeService.GenerateBarcodeValue();

        product.Barcode = newCode;
        product.BarcodeImage = _barcodeService.GenerateBarcode(newCode);

        await _uow.SaveChangesAsync();

        return Ok(new
        {
            Message = "Barcode regenerated successfully",
            product.Barcode
        });
    }


    [Authorize(Roles = "Admin,Pharmacist,Cashier")]
    [HttpGet("scan/{barcode}")]
    public async Task<IActionResult> Scan(string barcode)
    {
        var result = await _mediator.Send(new ScanBarcodeQuery
        {
            Barcode = barcode
        });

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }

    [Authorize(Roles = "Admin,Cashier")]
    [HttpPost("pos/add-to-cart")]
    public async Task<IActionResult> AddToCart(AddToCartCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }


    [Authorize(Roles = "Admin,Cashier")]
    [HttpPost("pos/checkout")]
    public async Task<IActionResult> Checkout(CheckoutCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }


    [Authorize(Roles = "Admin,Cashier,Pharmacist")]
    [HttpPost("scan-to-invoice")]
    public async Task<IActionResult> ScanToInvoice(ScanBarcodeToInvoiceCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }


    [Authorize(Roles = "Admin,Cashier")]
    [HttpDelete("{invoiceId}/items/{itemId}")]
    public async Task<IActionResult> RemoveItem(int invoiceId, int itemId)
    {
        var result = await _mediator.Send(new RemoveInvoiceItemCommand
        {
            InvoiceId = invoiceId,
            InvoiceItemId = itemId
        });

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }


    [Authorize(Roles = "Admin,Pharmacist")]
    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        var result = await _mediator.Send(new CancelInvoiceCommand
        {
            InvoiceId = id
        });

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }

    [Authorize(Roles = "Admin,Pharmacist")]
    [HttpGet("{id}/pdf")]
    public async Task<IActionResult> ExportPdf(int id)
    {
        var file = await _mediator.Send(new ExportInvoicePdfQuery
        {
            InvoiceId = id
        });

        return File(file, "application/pdf", $"invoice-{id}.pdf");
    }


    [Authorize(Roles = "Admin,Pharmacist")]
    [HttpGet("product/{id}/label")]
    public async Task<IActionResult> PrintLabel(int id)
    {
        var result = await _mediator.Send(new PrintBarcodeLabelQuery
        {
            ProductId = id
        });

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, result);

        return File(
            result.Data,
            "application/pdf",
            $"product-{id}-label.pdf");
    }


    [Authorize(Roles = "Admin,Pharmacist,Cashier")]
    [HttpGet("invoice/{id}/pdf")]
    public async Task<IActionResult> PrintInvoice(int id)
    {
        var file = await _mediator.Send(new ExportInvoicePdfQuery
        {
            InvoiceId = id
        });

        return File(file, "application/pdf", $"invoice-{id}.pdf");
    }
}