using SolveIt.Domain.Common;
using SolveIt.Domain.Exceptions;

namespace SolveIt.Domain.Organizers;

public sealed class Organizer
{
    public Guid Id { get; private set; }

    public string Email { get; private set; } = null!;

    public string Name { get; private set; } = null!;

    public string PhoneNumber { get; private set; } = null!;

    public string PasswordHash { get; private set; } = null!;

    public string AuthProvider { get; private set; } = null!;

    public DateTime CreatedAt { get; private set; }

    public UserRole Role { get; private set; }

    public bool IsBlocked { get; private set; }

    // Lockout
    public int FailedLoginAttempts { get; private set; }

    public DateTime? LockoutEndUtc { get; private set; }

    // Free trial tracking
    public bool HasUsedFreeTrial { get; private set; }

    public DateTime? FreeTrialUsedAtUtc { get; private set; }

    // Concurrency
    public byte[] RowVersion { get; private set; } = null!;

    private const int MaxFailedAttempts = 5;

    private static readonly TimeSpan LockoutDuration =
        TimeSpan.FromMinutes(15);

    private Organizer() { }

    private Organizer(
        Guid id,
        string name,
        string email,
        string phoneNumber,
        string passwordHash,
        string authProvider,
        UserRole role)
    {
        Id = id;
        Name = name;
        Email = email;
        PhoneNumber = phoneNumber;
        PasswordHash = passwordHash;
        AuthProvider = authProvider;
        Role = role;

        CreatedAt = DateTime.UtcNow;

        FailedLoginAttempts = 0;
        LockoutEndUtc = null;

        IsBlocked = false;

        HasUsedFreeTrial = false;
        FreeTrialUsedAtUtc = null;
    }

    public static Organizer Create(
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

        return new Organizer(
            Guid.NewGuid(),
            name.Trim(),
            email.Trim().ToLowerInvariant(),
            phoneNumber.Trim(),
            passwordHash,
            "local",
            UserRole.Organizer);
    }

    public void UpdateProfile(
    string name,
    string email,
    string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new NameRequiredException();

        if (string.IsNullOrWhiteSpace(email))
            throw new EmailRequiredException();

        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new PhoneNumberRequiredException();

        Name = name.Trim();
        Email = email.Trim().ToLowerInvariant();
        PhoneNumber = phoneNumber.Trim();
    }

    public void UpdatePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new InvalidPasswordHashException();

        PasswordHash = newPasswordHash;
    }

    public void Block()
    {
        IsBlocked = true;
    }

    public void Unblock()
    {
        IsBlocked = false;
    }

    // Login failure tracking
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

    // Free trial logic
    public void MarkFreeTrialUsed()
    {
        if (HasUsedFreeTrial)
            throw new InvalidOperationException("Free trial already used.");

        HasUsedFreeTrial = true;
        FreeTrialUsedAtUtc = DateTime.UtcNow;
    }
}