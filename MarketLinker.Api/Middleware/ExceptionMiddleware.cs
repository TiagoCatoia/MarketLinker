using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using MarketLinker.Domain.Exceptions;
using Microsoft.IdentityModel.Tokens;

namespace MarketLinker.Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    public ExceptionMiddleware(RequestDelegate next) => _next = next;
    
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = exception switch
        {
            NotFoundException => StatusCodes.Status404NotFound,
            ConflictException => StatusCodes.Status409Conflict,
            ArgumentException => StatusCodes.Status400BadRequest,
            SecurityTokenException => StatusCodes.Status401Unauthorized,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            ValidationException => StatusCodes.Status400BadRequest,
            InvalidOperationException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        var response = new
        {
            statusCode,
            message = exception.Message
        };

        var payload = JsonSerializer.Serialize(response);
        
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;
        
        return context.Response.WriteAsync(payload);
    }
}