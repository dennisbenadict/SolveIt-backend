namespace SolveIt.Application.Messaging;

public sealed record SubmissionRequested(
    Guid SubmissionId,
    Guid ProblemId
);
