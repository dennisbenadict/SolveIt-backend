using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solvelt.Domain.Organizers;

public sealed class Organizer
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string PhoneNumber { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public string AuthProvider { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }

    private Organizer() { } // EF Core only

    private Organizer(Guid id, string name, string email, string phoneNumber, string passwordHash, string authProvider)
    {
        Id = id;
        Name = name;
        Email = email;
        PhoneNumber = phoneNumber;
        PasswordHash = passwordHash;
        AuthProvider = authProvider;
        CreatedAt = DateTime.UtcNow;
    }

    public static Organizer Create(
        string name,
        string email,
        string phoneNumber,
        string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name required");

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email required");

        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone required");

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password required");


        return new Organizer(
            Guid.NewGuid(),
            email.Trim().ToLowerInvariant(),
            name.Trim(),
            phoneNumber,
            passwordHash,
            "local"
        );
    }
}

