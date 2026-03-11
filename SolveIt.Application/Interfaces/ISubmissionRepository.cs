using SolveIt.Domain.Submissions;

namespace SolveIt.Application.Interfaces;

public interface ISubmissionRepository
{
    Task AddAsync(
        Submission submission,
        CancellationToken cancellationToken);

    Task<Submission?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<bool> ExistsAsync(
        Guid problemId,
        Guid participantId,
        CancellationToken cancellationToken);

    Task<List<Submission>> GetByParticipantAsync(
    Guid participantId,
    CancellationToken cancellationToken);

    Task<List<Submission>> GetAcceptedByTournamentAsync(
    Guid tournamentId,
    CancellationToken cancellationToken);
}
