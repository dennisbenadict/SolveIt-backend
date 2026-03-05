using MediatR;
using SolveIt.Domain.Tournaments;

namespace SolveIt.Application.Tournaments.Queries.GetTournamentById;

public sealed record GetTournamentByIdQuery(
    Guid TournamentId
) : IRequest<Tournament>;
