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
}
