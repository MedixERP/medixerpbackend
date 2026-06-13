using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyERP.Application.Features.Product.Query;
using PharmacyERP.Application.Common.Models;

namespace PharmacyERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    
    [HttpPost]
    [Authorize(Roles = "Admin,Pharmacist")]
    public async Task<IActionResult> Add(AddProductCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }

    
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Pharmacist")]
    public async Task<IActionResult> Update(int id, UpdateProductCommand command)
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
        var result = await _mediator.Send(new DeleteProductCommand { Id = id });

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }

   
    [HttpGet]
    [Authorize(Roles = "Admin,Pharmacist")]
    public async Task<IActionResult> GetAll(
     [FromQuery] GetAllProductsQuery query)
    {
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }

   
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Pharmacist")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(
            new GetProductByIdQuery { Id = id });

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }

  
    [HttpGet("low-stock")]
    [Authorize(Roles = "Admin,Pharmacist")]
    public async Task<IActionResult> GetLowStock()
    {
        var result = await _mediator.Send(new GetLowStockProductsQuery());

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }

    // =======================
    // Search
    // =======================
    [HttpGet("search")]
    [Authorize(Roles = "Admin,Pharmacist")]
    public async Task<IActionResult> Search([FromQuery] string keyword)
    {
        var result = await _mediator.Send(
            new SearchProductsQuery { Keyword = keyword });

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }
}