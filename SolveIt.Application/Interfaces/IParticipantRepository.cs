using SolveIt.Domain.Participants;

namespace SolveIt.Application.Interfaces
{
    public interface IParticipantRepository
    {
        Task AddAsync(Participant participant, CancellationToken cancellationToken);
        Task<Participant?> GetByEmailAsync(string email, CancellationToken cancellationToken);
        Task<Participant?> GetByPhoneAsync(string phoneNumber, CancellationToken cancellationToken);
        Task<Participant?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
        Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken);
    }
}
