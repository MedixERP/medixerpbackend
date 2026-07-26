using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

[ApiController]
[Route("api/dashboard")]
[Authorize]
[EnableRateLimiting("heavy")]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("summary")]
    [Authorize(Roles = "Admin,Pharmacist,Cashier")]
    public async Task<IActionResult> Summary()
        => Ok(await _mediator.Send(new GetDashboardQuery()));

    [HttpGet("top-products")]
    [Authorize(Roles = "Admin,Pharmacist")]
    public async Task<IActionResult> TopProducts(int count = 5)
        => Ok(await _mediator.Send(new GetTopSellingQuery { Count = count }));

    [HttpGet("product-status")]
    [Authorize(Roles = "Admin,Pharmacist")]
    public async Task<IActionResult> ProductStatus()
        => Ok(await _mediator.Send(new GetProductStatusQuery()));

    [HttpGet("monthly-sales")]
    [Authorize(Roles = "Admin,Cashier")]
    public async Task<IActionResult> MonthlySales()
        => Ok(await _mediator.Send(new GetMonthlySalesQuery()));

    [HttpGet("inventory-value")]
    [Authorize(Roles = "Admin,Pharmacist")]
    public async Task<IActionResult> InventoryValue()
        => Ok(await _mediator.Send(new GetInventoryValueQuery()));

    [HttpGet("sales-vs-returns")]
    [Authorize(Roles = "Admin,Pharmacist")]
    public async Task<IActionResult> SalesVsReturns()
        => Ok(await _mediator.Send(new GetSalesVsReturnsQuery()));

    [HttpGet("profit")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Profit()
    {
        return Ok(
            await _mediator.Send(
                new GetProfitReportQuery()));
    }

    [HttpGet("today-sales")]
    [Authorize(Roles = "Admin,Cashier")]
    public async Task<IActionResult> TodaySales()
    {
        return Ok(
            await _mediator.Send(
                new GetTodaySalesQuery()));
    }

    [HttpGet("alerts")]
    [Authorize(Roles = "Admin,Pharmacist")]
    public async Task<IActionResult> Alerts()
    {
        return Ok(
            await _mediator.Send(
                new GetAlertsQuery()));
    }
}