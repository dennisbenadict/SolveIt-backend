using SolveIt.Domain.Exceptions;

public sealed class PasswordResetToken
{
    public Guid Id { get; private set; }
    public Guid OrganizerId { get; private set; }
    public string TokenHash { get; private set; } = default!;
    public DateTime ExpiresAtUtc { get; private set; }
    public bool IsUsed { get; private set; }

    private PasswordResetToken() { }

    private PasswordResetToken(
        Guid organizerId,
        string tokenHash,
        DateTime expiresAtUtc)
    {
        Id = Guid.NewGuid();
        OrganizerId = organizerId;
        TokenHash = tokenHash;
        ExpiresAtUtc = expiresAtUtc;
        IsUsed = false;
    }

    public static PasswordResetToken Create(
        Guid organizerId,
        string tokenHash,
        DateTime expiresAtUtc)
    {
        if (expiresAtUtc <= DateTime.UtcNow)
            throw new InvalidPasswordResetExpiryException();

        return new PasswordResetToken(
            organizerId,
            tokenHash,
            expiresAtUtc);
    }

    public void Use()
    {
        if (IsUsed)
            throw new PasswordResetTokenAlreadyUsedException();

        if (DateTime.UtcNow > ExpiresAtUtc)
            throw new PasswordResetTokenExpiredException();

        IsUsed = true;
    }
}


