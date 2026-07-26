using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyERP.Application.Features.SupplierPayments.Command;

namespace PharmacyERP.Controllers;

[ApiController]
[Route("api/supplier-payments")]
[Authorize(Roles = "Admin")]
public class SupplierPaymentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SupplierPaymentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Add(AddSupplierPaymentCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess
            ? Ok(result)
            : StatusCode(result.StatusCode, result);
    }

    [HttpGet("supplier/{supplierId}/debt")]
    public async Task<IActionResult> GetDebt(int supplierId)
    {
        var result = await _mediator.Send(
            new GetSupplierDebtQuery { SupplierId = supplierId });
        return result.IsSuccess
            ? Ok(result)
            : StatusCode(result.StatusCode, result);
    }
}