using MediatR;

namespace SolveIt.Application.Tournaments.Commands.CreateTournament;

public sealed record CreateTournamentCommand(
    Guid OrganizerId,
    string Title,
    string? DeviceFingerprint,
    string? IpAddress
) : IRequest<Guid>;
