using SolveIt.Application.Common.Interfaces;

namespace SolveIt.Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly SolveItDbContext _dbContext;

    public UnitOfWork(SolveItDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}

