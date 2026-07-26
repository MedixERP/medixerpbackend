using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PharmacyERP.Controllers;

[ApiController]
[Route("api/drug-orders")]
[Authorize]
public class DrugOrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public DrugOrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateDrugOrderCommand command)
    {
        var result = await _mediator.Send(command);

        return result.IsSuccess
            ? Ok(result)
            : StatusCode(result.StatusCode, result);
    }

    
    [HttpGet]
    [Authorize(Roles = "Admin,Pharmacist,PharmacyCompany")]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetAllDrugOrdersQuery query)
    {
        var result = await _mediator.Send(query);

        return result.IsSuccess
            ? Ok(result)
            : StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Pharmacist,PharmacyCompany")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(
            new GetDrugOrderByIdQuery
            {
                Id = id
            });

        return result.IsSuccess
            ? Ok(result)
            : StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id}/accept")]
    [Authorize(Roles = "PharmacyCompany")]
    public async Task<IActionResult> Accept(int id)
    {
        var result = await _mediator.Send(
            new AcceptDrugOrderCommand
            {
                OrderId = id
            });

        return result.IsSuccess
            ? Ok(result)
            : StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id}/reject")]
    [Authorize(Roles = "PharmacyCompany")]
    public async Task<IActionResult> Reject(
        int id,
        RejectDrugOrderCommand command)
    {
        command.OrderId = id;

        var result = await _mediator.Send(command);

        return result.IsSuccess
            ? Ok(result)
            : StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "PharmacyCompany")]
    public async Task<IActionResult> UpdateStatus(
        int id,
        UpdateDrugOrderStatusCommand command)
    {
        command.OrderId = id;

        var result = await _mediator.Send(command);

        return result.IsSuccess
            ? Ok(result)
            : StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id}/confirm-receipt")]
    [Authorize(Roles = "Pharmacist")]
    public async Task<IActionResult> ConfirmReceipt(int id)
    {
        var result = await _mediator.Send(
            new ConfirmDrugOrderReceiptCommand
            {
                OrderId = id
            });

        return result.IsSuccess
            ? Ok(result)
            : StatusCode(result.StatusCode, result);
    }
}