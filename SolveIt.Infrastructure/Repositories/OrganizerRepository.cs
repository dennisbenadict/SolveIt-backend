using Microsoft.EntityFrameworkCore;
using SolveIt.Infrastructure.Persistence;
using Solvelt.Application.Interfaces;
using Solvelt.Domain.Organizers;

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

    public async Task<bool> ExistsByPhoneAsync(
    string phoneNumber,
    CancellationToken cancellationToken)
    {
        return await _dbContext.Organizers
            .AnyAsync(o => o.PhoneNumber == phoneNumber, cancellationToken);
    }

    public async Task<Organizer?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await _dbContext.Organizers
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Email == email, cancellationToken);
    }

    public async Task<Organizer?> GetByIdAsync(
    Guid organizerId,
    CancellationToken cancellationToken)
    {
        return await _dbContext.Organizers
            .SingleOrDefaultAsync(x => x.Id == organizerId, cancellationToken);
    }

    public async Task<Organizer?> GetByPhoneAsync(string phone, CancellationToken cancellationToken)
    {
        return await _dbContext.Organizers
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.PhoneNumber == phone, cancellationToken);
    }

}

