using MediatR;

namespace SolveIt.Application.Tournaments.Commands.JoinTournament;

public sealed record JoinTournamentCommand(
    Guid TournamentId,
    Guid ParticipantId
) : IRequest;
