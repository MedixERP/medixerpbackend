using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyERP.Application.Common.Models;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    
    [Authorize(Roles = "Admin")]
    [HttpPost("register")]
    public async Task<ActionResult<Result<string>>> Register(RegisterCommand command)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _mediator.Send(command);

        return result.IsSuccess
            ? Ok(result)
            : StatusCode(result.StatusCode, result);
    }

    
    [HttpPost("login")]
    public async Task<ActionResult<Result<string>>> Login(LoginCommand command)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _mediator.Send(command);

        return result.IsSuccess
            ? Ok(result)
            : StatusCode(result.StatusCode, result);
    }
}