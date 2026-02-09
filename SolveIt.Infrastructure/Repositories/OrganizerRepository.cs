using Microsoft.EntityFrameworkCore;
using SolveIt.Infrastructure.Persistence;
using Solvelt.Application.Interfaces;
using Solvelt.Domain.Organizers;
using Solvelt.Infrastructure.Persistence;

namespace Solvelt.Infrastructure.Repositories;

public sealed class OrganizerRepository : IOrganizerRepository
{
    private readonly SolveItDbContext _dbContext;

    public OrganizerRepository(SolveItDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(
        Organizer organizer,
        CancellationToken cancellationToken)
    {
        await _dbContext.Organizers.AddAsync(organizer, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(
        string email,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Organizers
            .AnyAsync(o => o.Email == email, cancellationToken);
    }
}

