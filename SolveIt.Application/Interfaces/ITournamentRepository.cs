using SolveIt.Domain.Tournaments;

namespace SolveIt.Application.Interfaces;

public interface ITournamentRepository
{
    Task AddAsync(Tournament tournament, CancellationToken cancellationToken);

    Task<Tournament?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
