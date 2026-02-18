using Solvelt.Domain.Organizers;
using Solvelt.Application.Organizers.Commands.LoginOrganizer;

namespace Solvelt.Application.Common.Interfaces;

public interface IJwtTokenService
{
    string GenerateAccessToken(Organizer organizer);
    string GenerateRefreshToken();
    string HashRefreshToken(string refreshToken);
}

