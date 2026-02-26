using System.Security.Claims;
using SolveIt.Domain.Common;

namespace SolveIt.Api.Common.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (value is null || !Guid.TryParse(value, out var id))
            throw new UnauthorizedAccessException();

        return id;
    }
    public static string GetEmail(this ClaimsPrincipal user)
    {
        var value = user.FindFirst(ClaimTypes.Email)?.Value;

        if (string.IsNullOrWhiteSpace(value))
            throw new UnauthorizedAccessException("Email claim missing.");

        return value;
    }

    public static string GetName(this ClaimsPrincipal user)
    {
        var value = user.FindFirst("name")?.Value;

        if (string.IsNullOrWhiteSpace(value))
            throw new UnauthorizedAccessException("Name claim missing.");

        return value;
    }

    public static UserRole GetRole(this ClaimsPrincipal user)
    {
        var value = user.FindFirst(ClaimTypes.Role)?.Value;

        if (string.IsNullOrWhiteSpace(value))
            throw new UnauthorizedAccessException("Role claim missing.");

        if (!Enum.TryParse<UserRole>(value, out var role))
            throw new UnauthorizedAccessException();

        return role;
    }
}
