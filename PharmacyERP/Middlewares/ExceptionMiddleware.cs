using PharmacyERP.Application.Common.Exceptions;
using PharmacyERP.Application.Common.Models;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BusinessException ex)
        {
            context.Response.StatusCode = ex.StatusCode;

            await context.Response.WriteAsJsonAsync(
                Result<string>.Failure(ex.Message, ex.StatusCode));
        }
        catch (Exception)
        {
            context.Response.StatusCode = 500;

            await context.Response.WriteAsJsonAsync(
                Result<string>.Failure("Internal Server Error", 500));
        }
    }
}