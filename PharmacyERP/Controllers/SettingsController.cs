using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyERP.Application.Common.Models;

namespace PharmacyERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SettingsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SettingsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var result = await _mediator.Send(new GetProfileQuery());

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }

   
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile(UpdateProfileCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }

   
    [HttpGet("preferences")]
    public async Task<IActionResult> GetPreferences()
    {
        var result = await _mediator.Send(new GetPreferencesQuery());

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }

    
    [HttpPut("preferences")]
    public async Task<IActionResult> UpdatePreferences(UpdatePreferencesCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }

    
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }

    
    [HttpPost("avatar")]
    public async Task<IActionResult> UploadAvatar([FromForm] UploadAvatarCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }
}