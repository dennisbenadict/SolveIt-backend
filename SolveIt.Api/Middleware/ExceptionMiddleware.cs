using SolveIt.Application.Common.Exceptions;
using SolveIt.Api.Contracts;
using SolveIt.Domain.Exceptions;
using System.Net;
using FluentValidation;
namespace SolveIt.Api.Middleware;

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
        // FluentValidation exceptions
        catch (ValidationException ex)
        {
            await HandleValidationAsync(context, ex);
        }
        // Application-level exceptions
        catch (SolveIt.Application.Common.Exceptions.DomainException ex)
        {
            await HandleAsync(context, HttpStatusCode.BadRequest, ex.Message);
        }
        // Domain-level exceptions
        catch (SolveIt.Domain.Exceptions.DomainException ex)
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

    private static async Task HandleValidationAsync(
    HttpContext context,
    ValidationException ex)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        var errors = ex.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray());

        var response = ApiResponse<object>.Fail(
            errors
                .SelectMany(kvp => kvp.Value)
                .ToList(),
            "Validation failed",
            HttpStatusCode.BadRequest);

        await context.Response.WriteAsJsonAsync(response);
    }
}

