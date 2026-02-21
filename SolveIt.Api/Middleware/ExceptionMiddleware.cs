using SolveIt.Application.Common.Exceptions;
using Solvelt.Api.Contracts;
using Solvelt.Domain.Exceptions;
using System.Net;

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
        // Application-level exceptions
        catch (SolveIt.Application.Common.Exceptions.DomainException ex)
        {
            await HandleAsync(context, HttpStatusCode.BadRequest, ex.Message);
        }
        // Domain-level exceptions
        catch (Solvelt.Domain.Exceptions.DomainException ex)
        {
            await HandleAsync(context, HttpStatusCode.BadRequest, ex.Message);
        }
        // Unexpected
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

        var response = ApiResponse<object>.Fail(
            new List<string> { message },
            "Request failed",
            statusCode);

        await context.Response.WriteAsJsonAsync(response);
    }
}

