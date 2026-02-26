using SolveIt.Domain.Common;

namespace SolveIt.Application.Common.Interfaces;

public interface IJwtTokenService
{
    string GenerateAccessToken(
        Guid userId,
        string email,
        string name,
        UserRole role);
    string GenerateRefreshToken();
    string HashRefreshToken(string refreshToken);
}

