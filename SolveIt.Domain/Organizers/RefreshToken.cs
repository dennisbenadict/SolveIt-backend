using SolveIt.Domain.Exceptions;

namespace SolveIt.Domain.Organizers;

public sealed class RefreshToken
{
    public Guid Id { get; private set; }
    /// <summary>
    /// Represents the authenticated user's Id (Organizer or Participant).
    /// Originally tied to Organizer, now used as a generic user identifier.
    /// No foreign key constraint is enforced.
    /// </summary>
    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = null!;
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime ExpiresAtUtc { get; private set; }
    public bool IsRevoked { get; private set; }
    public DateTime? RevokedAtUtc { get; private set; }
    public Guid? ReplacedByTokenId { get; private set; }
    public string? DeviceFingerprint { get; private set; }
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }

    private RefreshToken() { } // EF Core

    private RefreshToken(
        Guid id,
        Guid userId,
        string tokenHash,
        DateTime createdAtUtc,
        DateTime expiresAtUtc,
        string? deviceFingerprint,
        string? ipAddress,
        string? userAgent)
    {
        Id = id;
        UserId = userId;
        TokenHash = tokenHash;
        CreatedAtUtc = createdAtUtc;
        ExpiresAtUtc = expiresAtUtc;
        DeviceFingerprint = deviceFingerprint;
        IpAddress = ipAddress;
        UserAgent = userAgent;
    }

    public static RefreshToken Create(
        Guid userId,
        string tokenHash,
        DateTime expiresAtUtc,
        string? deviceFingerprint,
        string? ipAddress,
        string? userAgent)
    {
        if (userId == Guid.Empty)
            throw new InvalidRefreshTokenOrganizerException();

        if (string.IsNullOrWhiteSpace(tokenHash))
            throw new InvalidRefreshTokenHashException();

        if (expiresAtUtc <= DateTime.UtcNow)
            throw new InvalidRefreshTokenExpiryException();

        return new RefreshToken(
            Guid.NewGuid(),
            userId,
            tokenHash,
            DateTime.UtcNow,
            expiresAtUtc,
            deviceFingerprint,
            ipAddress,
            userAgent
        );
    }

    public void Revoke(Guid? replacedByTokenId = null)
    {
        if (IsRevoked)
            return;

        IsRevoked = true;
        RevokedAtUtc = DateTime.UtcNow;
        ReplacedByTokenId = replacedByTokenId;
    }

    public bool IsExpired()
    {
        return DateTime.UtcNow >= ExpiresAtUtc;
    }
}


