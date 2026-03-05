using SolveIt.Domain.TournamentProblems;

namespace SolveIt.Application.Interfaces;

public interface ITournamentProblemRepository
{
    Task AddAsync(
        TournamentProblem entity,
        CancellationToken cancellationToken);

    Task<List<TournamentProblem>> GetByTournamentIdAsync(
        Guid tournamentId,
        CancellationToken cancellationToken);
}
