using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PharmacyERP.Controllers;

[ApiController]
[Route("api/customer-payments")]
[Authorize(Roles = "Admin,Cashier")]
public class CustomerPaymentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomerPaymentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Add(AddInvoicePaymentCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess
            ? Ok(result)
            : StatusCode(result.StatusCode, result);
    }

    [HttpGet("customer/{customerId}/debts")]
    public async Task<IActionResult> GetDebts(int customerId)
    {
        var result = await _mediator.Send(
            new GetCustomerDebtsQuery { CustomerId = customerId });
        return result.IsSuccess
            ? Ok(result)
            : StatusCode(result.StatusCode, result);
    }
}