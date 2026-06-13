using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/returns")]
[Authorize]
public class SalesReturnsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SalesReturnsController(IMediator mediator)
    {
        _mediator = mediator;
    }


    [HttpPost]
    [Authorize(Roles = "Admin,Pharmacist,Cashier")]
    public async Task<IActionResult> Create(CreateSalesReturnCommand command)
    {
        var result = await _mediator.Send(command);

        return result.IsSuccess
            ? Ok(result)
            : StatusCode(result.StatusCode, result);
    }


    [HttpGet]
    [Authorize(Roles = "Admin,Pharmacist")]
    public async Task<IActionResult> GetAll([FromQuery] GetSalesReturnsQuery query)
        => Ok(await _mediator.Send(query));


   [HttpGet("{id}")]

    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetSalesReturnByIdQuery { Id = id });

        return result.IsSuccess
            ? Ok(result)
            : StatusCode(result.StatusCode, result);
    }

   
    [HttpGet("export")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Export([FromQuery] string format)
    {
        var file = await _mediator.Send(new ExportSalesReturnsQuery
        {
            Format = format
        });

        var contentType =
            format.ToLower() == "excel"
                ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                : "application/pdf";

        return File(file, contentType, $"sales-returns.{format}");
    }

    
    [HttpGet("my-returns")]
    [Authorize(Roles = "Admin,Pharmacist,Cashier,Customer")]
    public async Task<IActionResult> MyReturns()
        => Ok(await _mediator.Send(new GetUserReturnsReportQuery()));
}