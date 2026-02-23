using System.Security.Claims;

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
}
