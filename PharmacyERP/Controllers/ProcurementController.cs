using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProcurementController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProcurementController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Pharmacist")]
    public async Task<IActionResult> Create(
        CreatePurchaseOrderCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }

    [HttpPut("receive/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Receive(int id)
    {
        var result = await _mediator.Send(
            new ReceivePurchaseOrderCommand
            {
                PurchaseOrderId = id
            });

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Pharmacist")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(
            new GetAllPurchaseOrdersQuery());

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Pharmacist")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(
            new GetPurchaseOrderByIdQuery
            {
                Id = id
            });

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }
}