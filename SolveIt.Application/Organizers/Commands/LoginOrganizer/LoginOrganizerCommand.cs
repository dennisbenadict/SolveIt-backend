using MediatR;

namespace Solvelt.Application.Organizers.Commands.LoginOrganizer;

public sealed record LoginOrganizerCommand(
    string Identifier,
    string Password
) : IRequest<LoginResponse>;

