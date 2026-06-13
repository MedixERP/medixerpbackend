using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyERP.Application.Common.Models;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Cashier")]
    public async Task<ActionResult<Result<int>>> Add(
        AddCustomerCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Cashier")]
    public async Task<ActionResult<Result<string>>> Update(
        int id,
        UpdateCustomerCommand command)
    {
        if (id != command.Id)
            return BadRequest(
                Result<string>.Failure(
                    "Id mismatch",
                    400));

        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Cashier,Pharmacist")]
    public async Task<ActionResult<Result<List<CustomerDto>>>> GetAll()
    {
        var result =
            await _mediator.Send(
                new GetAllCustomersQuery());

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Cashier,Pharmacist")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(
            new GetCustomerByIdQuery
            {
                Id = id
            });

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }

    [HttpGet("{id}/history")]
    [Authorize(Roles = "Admin,Pharmacist")]
    public async Task<ActionResult<Result<CustomerPurchaseHistoryDto>>>
        GetPurchaseHistory(int id)
    {
        var result =
            await _mediator.Send(
                new GetCustomerPurchaseHistoryQuery
                {
                    CustomerId = id
                });

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }
}