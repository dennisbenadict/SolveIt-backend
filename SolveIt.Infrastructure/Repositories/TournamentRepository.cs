using Microsoft.EntityFrameworkCore;
using SolveIt.Application.Interfaces;
using SolveIt.Domain.Tournaments;
using SolveIt.Infrastructure.Persistence;

namespace SolveIt.Infrastructure.Repositories;

public sealed class TournamentRepository
    : ITournamentRepository
{
    private readonly SolveItDbContext _context;

    public TournamentRepository(SolveItDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        Tournament tournament,
        CancellationToken cancellationToken)
    {
        await _context.Tournaments
            .AddAsync(tournament, cancellationToken);
    }

    public async Task<Tournament?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _context.Tournaments
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<List<Tournament>> GetByOrganizerIdAsync(
    Guid organizerId,
    CancellationToken cancellationToken)
    {
        return await _context.Tournaments
            .Where(t => t.OrganizerId == organizerId)
            .OrderByDescending(t => t.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }
}
