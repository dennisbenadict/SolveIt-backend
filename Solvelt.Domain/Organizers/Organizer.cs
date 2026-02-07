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
    public string AuthProvider { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }

    private Organizer() { } // EF Core only

    public Organizer(Guid id, string email, string name, string authProvider)
    {
        Id = id;
        Email = email;
        Name = name;
        AuthProvider = authProvider;
        CreatedAt = DateTime.UtcNow;
    }
}

