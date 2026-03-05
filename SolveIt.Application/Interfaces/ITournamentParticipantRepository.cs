using SolveIt.Domain.TournamentParticipants;

namespace SolveIt.Application.Interfaces;

public interface ITournamentParticipantRepository
{
    Task<bool> ExistsAsync(
        Guid tournamentId,
        Guid participantId,
        CancellationToken cancellationToken);

    Task AddAsync(
        TournamentParticipant entity,
        CancellationToken cancellationToken);
}
