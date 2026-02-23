using Microsoft.EntityFrameworkCore;
using Npgsql;
using SolveIt.Application.Interfaces;
using SolveIt.Application.Organizers.Exceptions;
using SolveIt.Domain.Organizers;
using SolveIt.Infrastructure.Persistence;

namespace SolveIt.Infrastructure.Repositories;

public sealed class OrganizerRepository : IOrganizerRepository
{
    private readonly SolveItDbContext _dbContext;

    public OrganizerRepository(SolveItDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    //public async Task AddAsync(
    //    Organizer organizer,
    //    CancellationToken cancellationToken)
    //{
    //    await _dbContext.Organizers.AddAsync(organizer, cancellationToken);
    //    await _dbContext.SaveChangesAsync(cancellationToken);
    //}

    public async Task AddAsync(
    Organizer organizer,
    CancellationToken cancellationToken)
    {
        await _dbContext.Organizers
            .AddAsync(organizer, cancellationToken);

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
            when (ex.InnerException is PostgresException pg &&
                  pg.SqlState == "23505")
        {
            var constraint = pg.ConstraintName;

            if (constraint == "IX_organizers_email")
                throw new EmailAlreadyExistsException();

            if (constraint == "IX_organizers_phone_number")
                throw new PhoneAlreadyExistsException();

            throw;
        }
    }

    public async Task<bool> ExistsByEmailAsync(
        string email,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Organizers
            .AnyAsync(o => o.Email == email, cancellationToken);
    }

    public async Task<bool> ExistsByPhoneAsync(
    string phoneNumber,
    CancellationToken cancellationToken)
    {
        return await _dbContext.Organizers
            .AnyAsync(o => o.PhoneNumber == phoneNumber, cancellationToken);
    }

    public async Task<Organizer?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await _dbContext.Organizers
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Email == email, cancellationToken);
    }

    public async Task<Organizer?> GetByIdAsync(
    Guid organizerId,
    CancellationToken cancellationToken)
    {
        return await _dbContext.Organizers
            .SingleOrDefaultAsync(x => x.Id == organizerId, cancellationToken);
    }

    public async Task<Organizer?> GetByPhoneAsync(string phone, CancellationToken cancellationToken)
    {
        return await _dbContext.Organizers
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.PhoneNumber == phone, cancellationToken);
    }

}

