using SolveIt.Domain.Exceptions;

namespace SolveIt.Domain.Organizers;

public sealed class RefreshToken
{
    public Guid Id { get; private set; }
    public Guid OrganizerId { get; private set; }
    public string TokenHash { get; private set; } = null!;
    public DateTime ExpiresAtUtc { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public bool IsRevoked { get; private set; }

    private RefreshToken() { } // EF Core

    private RefreshToken(
        Guid id,
        Guid organizerId,
        string tokenHash,
        DateTime createdAtUtc,
        DateTime expiresAtUtc,
        bool isRevoked)
    {
        Id = id;
        OrganizerId = organizerId;
        TokenHash = tokenHash;
        CreatedAtUtc = createdAtUtc;
        ExpiresAtUtc = expiresAtUtc;
        IsRevoked = isRevoked;
    }

    public static RefreshToken Create(
        Guid organizerId,
        string tokenHash,
        DateTime expiresAtUtc)
    {
        if (organizerId == Guid.Empty)
            throw new InvalidRefreshTokenOrganizerException();

        if (string.IsNullOrWhiteSpace(tokenHash))
            throw new InvalidRefreshTokenHashException();

        if (expiresAtUtc <= DateTime.UtcNow)
            throw new InvalidRefreshTokenExpiryException();

        return new RefreshToken(
            Guid.NewGuid(),
            organizerId,
            tokenHash,
            DateTime.UtcNow,
            expiresAtUtc,
            false
        );
    }

    public void Revoke()
    {
        if (!IsRevoked)
            IsRevoked = true;
    }

    public bool IsExpired()
    {
        return DateTime.UtcNow >= ExpiresAtUtc;
    }
}


