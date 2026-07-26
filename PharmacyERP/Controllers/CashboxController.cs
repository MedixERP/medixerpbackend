using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PharmacyERP.Controllers;

[ApiController]
[Route("api/cashbox")]
[Authorize(Roles = "Admin")]
public class CashboxController : ControllerBase
{
    private readonly IMediator _mediator;

    public CashboxController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("balance")]
    public async Task<IActionResult> Balance()
    {
        var result = await _mediator.Send(new GetCashboxBalanceQuery());
        return result.IsSuccess
            ? Ok(result)
            : StatusCode(result.StatusCode, result);
    }

    [HttpGet("transactions")]
    public async Task<IActionResult> Transactions(
        [FromQuery] GetCashboxTransactionsQuery query)
    {
        var result = await _mediator.Send(query);
        return result.IsSuccess
            ? Ok(result)
            : StatusCode(result.StatusCode, result);
    }
}