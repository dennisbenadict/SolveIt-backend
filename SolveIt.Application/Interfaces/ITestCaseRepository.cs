using SolveIt.Domain.TestCases;

namespace SolveIt.Application.Interfaces;

public interface ITestCaseRepository
{
    Task AddAsync(
        TestCase entity,
        CancellationToken cancellationToken);

    Task<List<TestCase>> GetByProblemIdAsync(
        Guid problemId,
        CancellationToken cancellationToken);
}