using MediatR;
using SolveIt.Domain.Tournaments;

namespace SolveIt.Application.Tournaments.Queries.GetMyTournaments;

public sealed record GetMyTournamentsQuery(
    Guid OrganizerId
) : IRequest<List<Tournament>>;
