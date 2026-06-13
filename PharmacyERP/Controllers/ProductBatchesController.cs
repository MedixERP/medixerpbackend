using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyERP.Application.Common.Models;

namespace PharmacyERP.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductBatchesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductBatchesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Pharmacist")]
    public async Task<IActionResult> Add(AddProductBatchCommand command)
    {
        var result = await _mediator.Send(command);

        return result.IsSuccess
            ? Ok(Result<int>.Success(result.Data, "Batch added successfully"))
            : StatusCode(result.StatusCode, Result<int>.Failure(result.Message, result.StatusCode));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Pharmacist")]
    public async Task<IActionResult> Update(int id, UpdateProductBatchCommand command)
    {
        if (id != command.Id)
            return BadRequest(Result<Unit>.Failure("Id mismatch", 400));

        var result = await _mediator.Send(command);

        return result.IsSuccess
            ? Ok(Result<Unit>.Success(Unit.Value, "Batch updated successfully"))
            : StatusCode(result.StatusCode, Result<Unit>.Failure(result.Message, result.StatusCode));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0)
            return BadRequest(Result<Unit>.Failure("Invalid id", 400));

        var result = await _mediator.Send(new DeleteProductBatchCommand { Id = id });

        return result.IsSuccess
            ? Ok(Result<Unit>.Success(Unit.Value, "Batch deleted successfully"))
            : StatusCode(result.StatusCode, Result<Unit>.Failure(result.Message, result.StatusCode));
    }

    [HttpGet("product/{productId:int}")]
    [Authorize(Roles = "Admin,Pharmacist,Cashier")]
    public async Task<IActionResult> GetProductBatches(int productId)
    {
        if (productId <= 0)
            return BadRequest(Result<Unit>.Failure("Invalid product id", 400));

        var result = await _mediator.Send(new GetProductBatchesQuery
        {
            ProductId = productId
        });

        return result.IsSuccess
            ? Ok(Result<List<ProductBatchsDto>>.Success(result.Data, "Batches retrieved successfully"))
            : StatusCode(result.StatusCode, Result<List<ProductBatchsDto>>.Failure(result.Message, result.StatusCode));
    }

    [HttpGet("expired")]
    [Authorize(Roles = "Admin,Pharmacist")]
    public async Task<IActionResult> GetExpired()
    {
        var result = await _mediator.Send(new GetExpiredBatchesQuery());

        return Ok(Result<List<ProductBatchsDto>>.Success(result, "Expired batches retrieved"));
    }

    [HttpGet("near-expiry")]
    [Authorize(Roles = "Admin,Pharmacist")]
    public async Task<IActionResult> GetNearExpiry([FromQuery] int days = 30)
    {
        if (days <= 0)
            days = 30;

        var result = await _mediator.Send(new GetNearExpiryBatchesQuery
        {
            Days = days
        });

        return Ok(Result<List<ProductBatchsDto>>.Success(result, "Near expiry batches retrieved"));
    }

    [HttpGet("stock/{productId:int}")]
    [Authorize(Roles = "Admin,Pharmacist,Cashier")]
    public async Task<IActionResult> GetStock(int productId)
    {
        if (productId <= 0)
            return BadRequest(
                Result<int>.Failure("Invalid product id", 400));

        var result = await _mediator.Send(new GetBatchStockQuery
        {
            ProductId = productId
        });

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }
    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,Pharmacist,Cashier")]
    public async Task<IActionResult> GetById(int id)
    {
        if (id <= 0)
            return BadRequest(
                Result<ProductBatchsDto>.Failure(
                    "Invalid batch id",
                    400));

        var result = await _mediator.Send(
            new GetProductBatchByIdQuery
            {
                Id = id
            });

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }
}