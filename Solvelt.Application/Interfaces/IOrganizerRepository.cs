using Solvelt.Domain.Organizers;

namespace Solvelt.Application.Interfaces;

public interface IOrganizerRepository
{
    Task AddAsync(Organizer organizer, CancellationToken cancellationToken);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken);
}

