using Solvelt.Domain.Organizers;
using Solvelt.Application.Organizers.Commands.LoginOrganizer;

namespace Solvelt.Application.Common.Interfaces;

public interface IJwtTokenService
{
    LoginResponse GenerateToken(Organizer organizer);
}

