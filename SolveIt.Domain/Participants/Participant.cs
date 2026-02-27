using SolveIt.Domain.Common;
using SolveIt.Domain.Exceptions;

namespace SolveIt.Domain.Participants;

public sealed class Participant
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string PhoneNumber { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public DateTime CreatedAtUtc { get; private set; }
    public UserRole Role { get; private set; }

    // Lockout
    public int FailedLoginAttempts { get; private set; }
    public DateTime? LockoutEndUtc { get; private set; }
    public bool IsBlocked { get; private set; }

    public byte[] RowVersion { get; private set; } = null!;

    private const int MaxFailedAttempts = 5;
    private static readonly TimeSpan LockoutDuration =
        TimeSpan.FromMinutes(15);

    private Participant() { }

    private Participant(
        Guid id,
        string name,
        string email,
        string phoneNumber,
        string passwordHash,
        UserRole role)
    {
        Id = id;
        Name = name;
        Email = email;
        PhoneNumber = phoneNumber;
        PasswordHash = passwordHash;
        CreatedAtUtc = DateTime.UtcNow;
        Role = role;

        FailedLoginAttempts = 0;
        LockoutEndUtc = null;
        IsBlocked = false;
    }

    public static Participant Create(
        string name,
        string email,
        string phoneNumber,
        string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new NameRequiredException();

        if (string.IsNullOrWhiteSpace(email))
            throw new EmailRequiredException();

        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new PhoneNumberRequiredException();

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new PasswordHashRequiredException();

        return new Participant(
            Guid.NewGuid(),
            name.Trim(),
            email.Trim().ToLowerInvariant(),
            phoneNumber.Trim(),
            passwordHash,
            UserRole.Participant);
    }

    public void UpdatePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new InvalidPasswordHashException();

        PasswordHash = newPasswordHash;
    }

    public void RegisterFailedLogin()
    {
        if (IsLockedOut())
            return;

        FailedLoginAttempts++;

        if (FailedLoginAttempts >= MaxFailedAttempts)
        {
            LockoutEndUtc = DateTime.UtcNow.Add(LockoutDuration);
            FailedLoginAttempts = 0;
        }
    }

    public void RegisterSuccessfulLogin()
    {
        FailedLoginAttempts = 0;
        LockoutEndUtc = null;
    }

    public bool IsLockedOut()
    {
        if (LockoutEndUtc is null)
            return false;

        if (LockoutEndUtc <= DateTime.UtcNow)
        {
            LockoutEndUtc = null;
            return false;
        }

        return true;
    }

    public void Block()
    {
        if (IsBlocked)
            return;

        IsBlocked = true;
    }

    public void Unblock()
    {
        if (!IsBlocked)
            return;

        IsBlocked = false;
    }
}
