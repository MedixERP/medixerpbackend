using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace PharmacyERP.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [EnableRateLimiting("heavy")]
    public async Task<IActionResult> GetAll([FromQuery] GetAllUsersQuery query)
    {
        var result = await _mediator.Send(query);
        return result.IsSuccess
            ? Ok(result)
            : StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetUserByIdQuery { Id = id });
        return result.IsSuccess
            ? Ok(result)
            : StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id}/disable")]
    public async Task<IActionResult> Disable(int id)
    {
        var result = await _mediator.Send(new DisableUserCommand { Id = id });
        return result.IsSuccess
            ? Ok(result)
            : StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id}/enable")]
    public async Task<IActionResult> Enable(int id)
    {
        var result = await _mediator.Send(new EnableUserCommand { Id = id });
        return result.IsSuccess
            ? Ok(result)
            : StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id}/role")]
    public async Task<IActionResult> ChangeRole(int id, ChangeUserRoleCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return result.IsSuccess
            ? Ok(result)
            : StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _mediator.Send(new DeleteUserCommand { Id = id });
        return result.IsSuccess
            ? Ok(result)
            : StatusCode(result.StatusCode, result);
    }
}