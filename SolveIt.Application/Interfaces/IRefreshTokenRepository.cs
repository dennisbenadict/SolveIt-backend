using SolveIt.Domain.Organizers;

namespace SolveIt.Application.Common.Interfaces;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken token, CancellationToken cancellationToken);

    Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken cancellationToken);

    Task RevokeAllByOrganizerIdAsync(
    Guid organizerId,
    CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);

    Task<List<RefreshToken>> GetActiveTokensByOrganizerIdAsync(
    Guid organizerId,
    CancellationToken cancellationToken);
}

