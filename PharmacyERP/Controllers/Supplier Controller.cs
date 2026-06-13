using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PharmacyERP.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class SuppliersController : ControllerBase
{
    private readonly IMediator _mediator;

    public SuppliersController(IMediator mediator)
    {
        _mediator = mediator;
    }


    [HttpPost]
    [Authorize(Roles = "Admin,Pharmacist")]
    public async Task<IActionResult> Add(AddSupplierCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Pharmacist")]
    public async Task<IActionResult> Update(int id, UpdateSupplierCommand command)
    {
        command.Id = id;

        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _mediator.Send(
            new DeleteSupplierCommand
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
        var result = await _mediator.Send(
            new GetAllSuppliersQuery());

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }


    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Pharmacist")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(
            new GetSupplierByIdQuery
            {
                Id = id
            });

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }

    [HttpGet("{id}/batches")]
    [Authorize(Roles = "Admin,Pharmacist")]
    public async Task<IActionResult> GetWithBatches(int id)
    {
        var result = await _mediator.Send(
            new GetSupplierWithBatchesQuery
            {
                Id = id
            });

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }
}