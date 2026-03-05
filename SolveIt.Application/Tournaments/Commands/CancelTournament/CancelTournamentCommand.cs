using MediatR;

namespace SolveIt.Application.Tournaments.Commands.CancelTournament;

public sealed record CancelTournamentCommand(
    Guid TournamentId,
    Guid OrganizerId
) : IRequest;
