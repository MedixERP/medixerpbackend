using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PharmacyERP.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UnitsController : ControllerBase
{
    private readonly IMediator _mediator;

    public UnitsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Pharmacist")]
    public async Task<IActionResult> Add(AddUnitCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess
            ? Ok(result)
            : StatusCode(result.StatusCode, result);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Pharmacist,Cashier")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllUnitsQuery());
        return result.IsSuccess
            ? Ok(result)
            : StatusCode(result.StatusCode, result);
    }

    [HttpPost("product-unit")]
    [Authorize(Roles = "Admin,Pharmacist")]
    public async Task<IActionResult> AddProductUnit(
        AddProductUnitCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess
            ? Ok(result)
            : StatusCode(result.StatusCode, result);
    }

    [HttpGet("product/{productId}")]
    [Authorize(Roles = "Admin,Pharmacist,Cashier")]
    public async Task<IActionResult> GetProductUnits(int productId)
    {
        var result = await _mediator.Send(
            new GetProductUnitsQuery { ProductId = productId });
        return result.IsSuccess
            ? Ok(result)
            : StatusCode(result.StatusCode, result);
    }

    [HttpGet("convert")]
    [Authorize(Roles = "Admin,Pharmacist,Cashier")]
    public async Task<IActionResult> Convert(
        [FromQuery] ConvertUnitQuery query)
    {
        var result = await _mediator.Send(query);
        return result.IsSuccess
            ? Ok(result)
            : StatusCode(result.StatusCode, result);
    }
}