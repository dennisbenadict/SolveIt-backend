using Microsoft.EntityFrameworkCore;
using SolveIt.Application.Interfaces;
using SolveIt.Domain.Participants;
using SolveIt.Infrastructure.Persistence;

namespace SolveIt.Infrastructure.Repositories
{
    public sealed class ParticipantRepository : IParticipantRepository
    {
        private readonly SolveItDbContext _context;

        public ParticipantRepository(SolveItDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Participant participant, CancellationToken cancellationToken)
        {
            await _context.Participants.AddAsync(participant, cancellationToken);
        }

        public async Task<Participant?> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            return await _context.Participants
                .FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
        }

        public async Task<Participant?> GetByPhoneAsync(string phoneNumber, CancellationToken cancellationToken)
        {
            return await _context.Participants
                .FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber, cancellationToken);
        }

        public async Task<Participant?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Participants
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
