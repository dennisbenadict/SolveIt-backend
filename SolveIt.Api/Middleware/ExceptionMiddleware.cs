using SolveIt.Application.Common.Exceptions;
using Solvelt.Application.Organizers.Exceptions;
using System.Net;
using System.Text.Json;

namespace Solvelt.Api.Middleware;

public sealed class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (EmailAlreadyExistsException ex)
        {
            await HandleAsync(context, HttpStatusCode.Conflict, ex.Message);
        }
        catch (PhoneAlreadyExistsException ex)
        {
            await HandleAsync(context, HttpStatusCode.Conflict, ex.Message);
        }
        catch (DomainException ex)
        {
            await HandleAsync(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");

            await HandleAsync(
                context,
                HttpStatusCode.InternalServerError,
                "An unexpected error occurred.");
        }
    }

    private static async Task HandleAsync(
        HttpContext context,
        HttpStatusCode statusCode,
        string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            error = message
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response));
    }
}

