using MassTransit;
using Microsoft.EntityFrameworkCore;
using SolveIt.Application.Interfaces;
using SolveIt.Domain.Submissions;
using SolveIt.Infrastructure.Persistence;

namespace SolveIt.Infrastructure.Repositories;

public sealed class SubmissionRepository : ISubmissionRepository
{
    private readonly SolveItDbContext _context;

    public SubmissionRepository(SolveItDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        Submission submission,
        CancellationToken cancellationToken)
    {
        await _context.Submissions.AddAsync(submission, cancellationToken);
    }

    public async Task<Submission?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _context.Submissions
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<bool> ExistsAsync(
    Guid problemId,
    Guid participantId,
    CancellationToken cancellationToken)
    {
        return await _context.Submissions.AnyAsync(
            x => x.TournamentProblemId == problemId &&
                 x.ParticipantId == participantId,
            cancellationToken);
    }

    public async Task<List<Submission>> GetByParticipantAsync(
    Guid participantId,
    CancellationToken cancellationToken)
    {
        return await _context.Submissions
            .Where(x => x.ParticipantId == participantId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Submission>> GetAcceptedByTournamentAsync(
        Guid tournamentId,
        CancellationToken cancellationToken)
    {
        return await _context.Submissions
            .Include(s => s.TournamentProblem)
            .Where(s =>
                s.Status == SubmissionStatus.Accepted &&
                s.TournamentProblem.TournamentId == tournamentId)
            .ToListAsync(cancellationToken);
    }
}
