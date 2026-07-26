using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PharmacyERP.Controllers;

[ApiController]
[Route("api/expenses")]
[Authorize(Roles = "Admin")]
public class ExpensesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ExpensesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Add(AddExpenseCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess
            ? Ok(result)
            : StatusCode(result.StatusCode, result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetAllExpensesQuery query)
    {
        var result = await _mediator.Send(query);
        return result.IsSuccess
            ? Ok(result)
            : StatusCode(result.StatusCode, result);
    }
}