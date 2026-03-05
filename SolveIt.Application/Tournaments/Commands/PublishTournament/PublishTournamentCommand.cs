using MediatR;

namespace SolveIt.Application.Tournaments.Commands.PublishTournament;

public sealed record PublishTournamentCommand(
    Guid TournamentId,
    Guid OrganizerId
) : IRequest;