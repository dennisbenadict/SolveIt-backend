using MediatR;
using SolveIt.Application.Interfaces;
using SolveIt.Domain.Tournaments;

namespace SolveIt.Application.Tournaments.Queries.GetMyTournaments;

public sealed class GetMyTournamentsHandler
    : IRequestHandler<GetMyTournamentsQuery, List<Tournament>>
{
    private readonly ITournamentRepository _tournamentRepository;

    public GetMyTournamentsHandler(
        ITournamentRepository tournamentRepository)
    {
        _tournamentRepository = tournamentRepository;
    }

    public async Task<List<Tournament>> Handle(
        GetMyTournamentsQuery request,
        CancellationToken cancellationToken)
    {
        return await _tournamentRepository
            .GetByOrganizerIdAsync(
                request.OrganizerId,
                cancellationToken);
    }
}
