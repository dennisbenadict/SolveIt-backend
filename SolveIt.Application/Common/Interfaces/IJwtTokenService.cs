using SolveIt.Domain.Organizers;
using SolveIt.Application.Organizers.Commands.LoginOrganizer;

namespace SolveIt.Application.Common.Interfaces;

public interface IJwtTokenService
{
    string GenerateAccessToken(Organizer organizer);
    string GenerateRefreshToken();
    string HashRefreshToken(string refreshToken);
}

