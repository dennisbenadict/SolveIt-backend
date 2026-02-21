using Microsoft.EntityFrameworkCore;
using SolveIt.Infrastructure.Persistence;
using SolveIt.Application.Common.Interfaces;
using SolveIt.Domain.Organizers;

namespace SolveIt.Infrastructure.Repositories;

public sealed class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly SolveItDbContext _dbContext;

    public RefreshTokenRepository(SolveItDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(RefreshToken token, CancellationToken cancellationToken)
    {
        await _dbContext.RefreshTokens.AddAsync(token, cancellationToken);
    }

    public async Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken cancellationToken)
    {
        return await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);
    }

    public async Task RevokeAllByOrganizerIdAsync(
    Guid organizerId,
    CancellationToken cancellationToken)
    {
        await _dbContext.RefreshTokens
            .Where(x => x.OrganizerId == organizerId && !x.IsRevoked)
            .ExecuteUpdateAsync(
                s => s.SetProperty(p => p.IsRevoked, true),
                cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

