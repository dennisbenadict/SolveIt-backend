using Microsoft.EntityFrameworkCore;
using SolveIt.Application.Interfaces;
using SolveIt.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveIt.Infrastructure.Repositories
{
    public sealed class PasswordResetTokenRepository
        : IPasswordResetTokenRepository
    {
        private readonly SolveItDbContext _dbContext;

        public PasswordResetTokenRepository(SolveItDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(
            PasswordResetToken token,
            CancellationToken cancellationToken)
        {
            await _dbContext.PasswordResetTokens
                .AddAsync(token, cancellationToken);
        }

        public async Task<PasswordResetToken?> GetByHashAsync(
            string hash,
            CancellationToken cancellationToken)
        {
            return await _dbContext.PasswordResetTokens
                .FirstOrDefaultAsync(x => x.TokenHash == hash, cancellationToken);
        }

        public async Task SaveChangesAsync(
            CancellationToken cancellationToken)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task RevokeActiveTokensAsync(
            Guid organizerId,
            CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            await _dbContext.PasswordResetTokens
                .Where(x => x.OrganizerId == organizerId
                            && !x.IsUsed
                            && x.ExpiresAtUtc > now)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.IsUsed, true),
                    cancellationToken);
        }

        public async Task<int> CountRecentRequestsAsync(
            Guid organizerId,
            DateTime since,
            CancellationToken cancellationToken)
        {
            return await _dbContext.PasswordResetTokens
                .Where(x => x.OrganizerId == organizerId
                            && x.CreatedAtUtc >= since)
                .CountAsync(cancellationToken);
        }
    }

}
