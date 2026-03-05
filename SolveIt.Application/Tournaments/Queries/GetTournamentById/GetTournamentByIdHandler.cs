using MediatR;
using SolveIt.Application.Interfaces;
using SolveIt.Domain.Exceptions;
using SolveIt.Domain.Tournaments;

namespace SolveIt.Application.Tournaments.Queries.GetTournamentById;

public sealed class GetTournamentByIdHandler
    : IRequestHandler<GetTournamentByIdQuery, Tournament>
{
    private readonly ITournamentRepository _tournamentRepository;

    public GetTournamentByIdHandler(
        ITournamentRepository tournamentRepository)
    {
        _tournamentRepository = tournamentRepository;
    }

    public async Task<Tournament> Handle(
        GetTournamentByIdQuery request,
        CancellationToken cancellationToken)
    {
        var tournament =
            await _tournamentRepository.GetByIdAsync(
                request.TournamentId,
                cancellationToken);

        if (tournament is null)
            throw new TournamentNotFoundException();

        return tournament;
    }
}
