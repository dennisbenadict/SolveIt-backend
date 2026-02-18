using MediatR;
using Solvelt.Application.Organizers.Commands.LoginOrganizer;

namespace Solvelt.Application.Organizers.Commands.RefreshOrganizerToken;

public sealed record RefreshOrganizerTokenCommand(string RefreshToken)
    : IRequest<LoginResponse>;

