using Microsoft.AspNetCore.Http;
using PharmacyERP.Application.Common.Exceptions;
using System.Net;
using System.Text.Json;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BusinessException ex)
        {
            await HandleException(context, ex.Message, ex.StatusCode);
        }
        catch (Exception ex)
        {
            await HandleException(context, ex.Message, 500);
        }
    }

    private static async Task HandleException(HttpContext context, string message, int statusCode)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var response = new
        {
            success = false,
            message
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}