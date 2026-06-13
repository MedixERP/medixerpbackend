using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PharmacyERP.Controllers;

[ApiController]
[Route("api/export")]
[Authorize]
public class ExportController : ControllerBase
{
    private readonly IMediator _mediator;

    public ExportController(IMediator mediator)
    {
        _mediator = mediator;
    }

    
    [HttpGet("invoices")]
    [Authorize(Roles = "Admin,Pharmacist")]
    public async Task<IActionResult> ExportInvoices([FromQuery] string format = "pdf")
    {
        var file = await _mediator.Send(new ExportInvoicesQuery { Format = format });

        return File(file,
            format.ToLower() == "excel"
                ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                : "application/pdf",
            $"Invoices.{(format.ToLower() == "excel" ? "xlsx" : "pdf")}");
    }

   
    [HttpGet("products")]
    [Authorize(Roles = "Admin,Pharmacist")]
    public async Task<IActionResult> ExportProducts([FromQuery] string format = "pdf")
    {
        var file = await _mediator.Send(new ExportProductsQuery { Format = format });

        return File(file,
            format.ToLower() == "excel"
                ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                : "application/pdf",
            $"Products.{(format.ToLower() == "excel" ? "xlsx" : "pdf")}");
    }

    
    [HttpGet("purchase-orders")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ExportPurchaseOrders([FromQuery] string format = "pdf")
    {
        var file = await _mediator.Send(new ExportPurchaseOrdersQuery { Format = format });

        return File(file,
            format.ToLower() == "excel"
                ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                : "application/pdf",
            $"PurchaseOrders.{(format.ToLower() == "excel" ? "xlsx" : "pdf")}");
    }

    
    [HttpGet("customers")]
    [Authorize(Roles = "Admin,Cashier")]
    public async Task<IActionResult> ExportCustomers([FromQuery] string format = "pdf")
    {
        var file = await _mediator.Send(new ExportCustomersQuery { Format = format });

        return File(file,
            format.ToLower() == "excel"
                ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                : "application/pdf",
            $"Customers.{(format.ToLower() == "excel" ? "xlsx" : "pdf")}");
    }

  
    [HttpGet("suppliers")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ExportSuppliers([FromQuery] string format = "pdf")
    {
        var file = await _mediator.Send(new ExportSuppliersQuery { Format = format });

        return File(file,
            format.ToLower() == "excel"
                ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                : "application/pdf",
            $"Suppliers.{(format.ToLower() == "excel" ? "xlsx" : "pdf")}");
    }

   
    [HttpGet("expired-batches")]
    [Authorize(Roles = "Admin,Pharmacist")]
    public async Task<IActionResult> ExportExpiredBatches([FromQuery] string format = "pdf")
    {
        var file = await _mediator.Send(new ExportExpiredBatchesQuery { Format = format });

        return File(file,
            format.ToLower() == "excel"
                ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                : "application/pdf",
            $"ExpiredBatches.{(format.ToLower() == "excel" ? "xlsx" : "pdf")}");
    }

   
    [HttpGet("low-stock")]
    [Authorize(Roles = "Admin,Pharmacist")]
    public async Task<IActionResult> ExportLowStock([FromQuery] string format = "pdf")
    {
        var file = await _mediator.Send(new ExportLowStockProductsQuery { Format = format });

        return File(file,
            format.ToLower() == "excel"
                ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                : "application/pdf",
            $"LowStockProducts.{(format.ToLower() == "excel" ? "xlsx" : "pdf")}");
    }

    
    [HttpGet("monthly-sales")]
    [Authorize(Roles = "Admin,Cashier")]
    public async Task<IActionResult> ExportMonthlySales([FromQuery] string format = "pdf")
    {
        var file = await _mediator.Send(new ExportMonthlySalesQuery { Format = format });

        return File(file,
            format.ToLower() == "excel"
                ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                : "application/pdf",
            $"MonthlySales.{(format.ToLower() == "excel" ? "xlsx" : "pdf")}");
    }
}