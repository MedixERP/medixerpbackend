using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyERP.Application.Common.Models;

namespace PharmacyERP.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class InvoicesController : ControllerBase
{
    private readonly IMediator _mediator;

    public InvoicesController(IMediator mediator)
    {
        _mediator = mediator;
    }


    [HttpPost]
    [Authorize(Roles = "Admin,Cashier")]
    public async Task<ActionResult<Result<int>>> Create(
          CreateInvoiceCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }

    [HttpPut("cancel/{id}")]
    [Authorize(Roles = "Admin,Pharmacist")]
    public async Task<ActionResult<Result<string>>> Cancel(int id)
    {
        var result = await _mediator.Send(
            new CancelInvoiceCommand
            {
                InvoiceId = id
            });

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Pharmacist,Cashier")]
    public async Task<ActionResult<Result<InvoiceDto>>> GetById(int id)
    {
        var result = await _mediator.Send(
            new GetInvoiceByIdQuery
            {
                Id = id
            });

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }


    [HttpGet]
    [Authorize(Roles = "Admin,Pharmacist")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllInvoicesQuery());

        return Ok(result);
    }

    [HttpGet("summary")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetSummary([FromQuery] DateTime date)
    {
        var result = await _mediator.Send(new GetSalesSummaryQuery
        {
            Date = date
        });

        return Ok(result);
    }

   
    [HttpGet("export/invoices")]
    [Authorize(Roles = "Admin,Pharmacist")]
    public async Task<IActionResult> ExportInvoices([FromQuery] string format)
    {
        if (string.IsNullOrWhiteSpace(format))
            return BadRequest("Format is required (excel or pdf)");

        format = format.ToLower();

        if (format != "excel" && format != "pdf")
            return BadRequest("Invalid format. Use 'excel' or 'pdf' only.");

        var file = await _mediator.Send(new ExportInvoicesQuery
        {
            Format = format
        });

        var contentType = format == "excel"
            ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            : "application/pdf";

        var fileName = format == "excel"
            ? "invoices.xlsx"
            : "invoices.pdf";

        return File(file, contentType, fileName);
    }

   
    [HttpGet("export/products")]
    [Authorize(Roles = "Admin,Pharmacist")]
    public async Task<IActionResult> ExportProducts([FromQuery] string format = "pdf")
    {
        var file = await _mediator.Send(new ExportProductsQuery
        {
            Format = format
        });

        return File(file,
            format.ToLower() == "excel"
                ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                : "application/pdf",
            $"products.{(format.ToLower() == "excel" ? "xlsx" : "pdf")}");
    }

  
    [HttpGet("movements")]
    [Authorize(Roles = "Admin,Pharmacist")]
    public async Task<IActionResult> GetMovements()
    {
        var result = await _mediator.Send(new GetInventoryMovementsQuery());
        return Ok(result);
    }
    [HttpGet("{id}/pdf")]
    [Authorize(Roles = "Admin,Pharmacist,Cashier")]
    public async Task<IActionResult> GetInvoicePdf(int id)
    {
        var file = await _mediator.Send(new ExportInvoicePdfQuery
        {
            InvoiceId = id
        });

        return File(file, "application/pdf", $"invoice-{id}.pdf");
    }
}