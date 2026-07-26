using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PharmacyERP.Controllers;

[ApiController]
[Route("api/stock-adjustment")]
[Authorize(Roles = "Admin,Pharmacist")]
public class StockAdjustmentController : ControllerBase
{
    private readonly IMediator _mediator;

    public StockAdjustmentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPut("{batchId}")]
    public async Task<IActionResult> Adjust(int batchId, AdjustStockCommand command)
    {
        command.BatchId = batchId;
        var result = await _mediator.Send(command);
        return result.IsSuccess
            ? Ok(result)
            : StatusCode(result.StatusCode, result);
    }
}