using MassTransit;
using Microsoft.EntityFrameworkCore;
using SolveIt.Application.Interfaces;
using SolveIt.Application.Messaging;
using SolveIt.Domain.Submissions;
using SolveIt.Infrastructure.Persistence;
using SolveIt.Worker.Execution;

namespace SolveIt.Worker.Consumers;

public sealed class SubmissionConsumer : IConsumer<SubmissionRequested>
{
    private readonly SolveItDbContext _db;
    private readonly ISubmissionRepository _submissionRepository;
    private readonly ITestCaseRepository _testCaseRepository;

    public SubmissionConsumer(
        SolveItDbContext db,
        ISubmissionRepository submissionRepository,
        ITestCaseRepository testCaseRepository)
    {
        _db = db;
        _submissionRepository = submissionRepository;
        _testCaseRepository = testCaseRepository;
    }

    public async Task Consume(ConsumeContext<SubmissionRequested> context)
    {
        var msg = context.Message;

        var submission = await _submissionRepository
            .GetByIdAsync(msg.SubmissionId, context.CancellationToken);

        if (submission is null)
            return;

        // mark running
        submission.MarkRunning();
        await _db.SaveChangesAsync(context.CancellationToken);

        // load test cases
        var testCases = await _testCaseRepository
            .GetByProblemIdAsync(msg.ProblemId, context.CancellationToken);

        int passed = 0;
        int total = testCases.Count;

        foreach (var tc in testCases)
        {
            var exec = await DockerRunner.ExecuteAsync(
                submission.Language.ToString().ToLowerInvariant(),
                submission.SourceCode,
                tc.Input,
                2000, // temporary time limit (ms)
                context.CancellationToken);

            if (exec.IsTimeout)
            {
                submission.MarkFailed(
                    SubmissionStatus.TimeLimitExceeded,
                    passed,
                    total);
                break;
            }

            if (!exec.IsSuccess)
            {
                submission.MarkFailed(
                    SubmissionStatus.RuntimeError,
                    passed,
                    total);
                break;
            }

            if (exec.Output.Trim() == tc.ExpectedOutput.Trim())
            {
                passed++;
            }
            else
            {
                submission.MarkFailed(
                    SubmissionStatus.WrongAnswer,
                    passed,
                    total);
                break;
            }
        }

        // update result
        if (passed == total)
        {
            submission.MarkAccepted(total);
        }
        else
        {
            submission.MarkFailed(
                SubmissionStatus.WrongAnswer,
                passed,
                total);
        }

        await _db.SaveChangesAsync(context.CancellationToken);
    }
}
