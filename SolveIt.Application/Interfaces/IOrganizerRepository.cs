using Solvelt.Domain.Organizers;

namespace Solvelt.Application.Interfaces;

public interface IOrganizerRepository
{
    Task AddAsync(Organizer organizer, CancellationToken cancellationToken);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken);
    Task<bool> ExistsByPhoneAsync(string phoneNumber, CancellationToken cancellationToken);
    Task<Organizer?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<Organizer?> GetByPhoneAsync(string phone, CancellationToken cancellationToken);
}
 
