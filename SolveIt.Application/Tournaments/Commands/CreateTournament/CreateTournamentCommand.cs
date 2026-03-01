using MediatR;

namespace SolveIt.Application.Tournaments.Commands.CreateTournament;

public sealed record CreateTournamentCommand(
    Guid OrganizerId,
    string Title,
    string? Description,
    string? DeviceFingerprint,
    string? IpAddress,
    DateTime StartTimeUtc,
    DateTime EndTimeUtc
) : IRequest<Guid>;
