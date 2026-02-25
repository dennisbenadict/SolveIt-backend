using Microsoft.AspNetCore.Authentication.JwtBearer;
using SolveIt.Api.Contracts;
using System.Net;

public sealed class JwtEventsHandler : JwtBearerEvents
{
    public override Task Challenge(JwtBearerChallengeContext context)
    {
        context.HandleResponse();
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;

        return context.Response.WriteAsJsonAsync(
            ApiResponse<string>.Fail(
                new[] { "Unauthorized" },
                "Authentication required",
                HttpStatusCode.Unauthorized));
    }

    public override Task Forbidden(ForbiddenContext context)
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;

        return context.Response.WriteAsJsonAsync(
            ApiResponse<string>.Fail(
                new[] { "Forbidden" },
                "You do not have permission to access this resource",
                HttpStatusCode.Forbidden));
    }
}
