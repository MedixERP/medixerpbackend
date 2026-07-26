using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PharmacyERP.Controllers;

[ApiController]
[Route("api/pharmacy-companies")]
[Authorize(Roles = "Admin")]
public class PharmacyCompaniesController : ControllerBase
{
    private readonly IMediator _mediator;

    public PharmacyCompaniesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Add(AddPharmacyCompanyCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess
            ? Ok(result)
            : StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        int id, UpdatePharmacyCompanyCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return result.IsSuccess
            ? Ok(result)
            : StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id}/disable")]
    public async Task<IActionResult> Disable(int id)
    {
        var result = await _mediator.Send(
            new DisablePharmacyCompanyCommand { Id = id });
        return result.IsSuccess
            ? Ok(result)
            : StatusCode(result.StatusCode, result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetAllPharmacyCompaniesQuery query)
    {
        var result = await _mediator.Send(query);
        return result.IsSuccess
            ? Ok(result)
            : StatusCode(result.StatusCode, result);
    }
}