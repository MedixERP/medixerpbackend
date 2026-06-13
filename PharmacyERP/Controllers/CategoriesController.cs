using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyERP.Application.Common.Models;
using PharmacyERP.Application.Features.Category.DTOs;

namespace PharmacyERP.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Result<int>>> Add(AddCategoryCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }

    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Result<string>>> Update(UpdateCategoryCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Result<string>>> Delete(int id)
    {
        var result = await _mediator.Send(new DeleteCategoryCommand
        {
            Id = id
        });

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Pharmacist,Cashier")]
    public async Task<ActionResult<Result<List<CategoryDto>>>> GetAll()
    {
        var result = await _mediator.Send(new GetAllCategoriesQuery());

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }
}