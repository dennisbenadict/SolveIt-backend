using Microsoft.EntityFrameworkCore;
using SolveIt.Application.Interfaces;
using SolveIt.Domain.TestCases;
using SolveIt.Infrastructure.Persistence;

namespace SolveIt.Infrastructure.Repositories;

public sealed class TestCaseRepository : ITestCaseRepository
{
    private readonly SolveItDbContext _context;

    public TestCaseRepository(SolveItDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        TestCase entity,
        CancellationToken cancellationToken)
    {
        await _context.TestCases.AddAsync(entity, cancellationToken);
    }

    public async Task<List<TestCase>> GetByProblemIdAsync(
        Guid problemId,
        CancellationToken cancellationToken)
    {
        return await _context.TestCases
            .Where(x => x.TournamentProblemId == problemId)
            .OrderBy(x => x.OrderIndex)
            .ToListAsync(cancellationToken);
    }
}