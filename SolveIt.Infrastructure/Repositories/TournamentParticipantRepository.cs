using Microsoft.EntityFrameworkCore;
using SolveIt.Application.Interfaces;
using SolveIt.Domain.TournamentParticipants;
using SolveIt.Infrastructure.Persistence;

namespace SolveIt.Infrastructure.Repositories;

public sealed class TournamentParticipantRepository
    : ITournamentParticipantRepository
{
    private readonly SolveItDbContext _context;

    public TournamentParticipantRepository(SolveItDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsAsync(
        Guid tournamentId,
        Guid participantId,
        CancellationToken cancellationToken)
    {
        return await _context.TournamentParticipants
            .AnyAsync(
                x => x.TournamentId == tournamentId &&
                     x.ParticipantId == participantId,
                cancellationToken);
    }

    public async Task AddAsync(
        TournamentParticipant entity,
        CancellationToken cancellationToken)
    {
        await _context.TournamentParticipants
            .AddAsync(entity, cancellationToken);
    }
}