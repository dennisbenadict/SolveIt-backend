namespace SolveIt.Domain.Submissions;

public sealed class Submission
{
    public Guid Id { get; private set; }

    public Guid TournamentProblemId { get; private set; }

    public Guid ParticipantId { get; private set; }

    public string Language { get; private set; } = null!;

    public string SourceCode { get; private set; } = null!;

    public SubmissionStatus Status { get; private set; }

    public int PassedTestCases { get; private set; }

    public int TotalTestCases { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public byte[] RowVersion { get; private set; } = null!;

    private Submission() { }

    private Submission(
        Guid id,
        Guid problemId,
        Guid participantId,
        string language,
        string sourceCode)
    {
        Id = id;
        TournamentProblemId = problemId;
        ParticipantId = participantId;
        Language = language;
        SourceCode = sourceCode;

        Status = SubmissionStatus.Pending;
        PassedTestCases = 0;
        TotalTestCases = 0;

        CreatedAtUtc = DateTime.UtcNow;
    }

    public static Submission Create(
        Guid problemId,
        Guid participantId,
        string language,
        string sourceCode)
    {
        if (string.IsNullOrWhiteSpace(language))
            throw new ArgumentException("Language required.");

        if (string.IsNullOrWhiteSpace(sourceCode))
            throw new ArgumentException("Source code required.");

        return new Submission(
            Guid.NewGuid(),
            problemId,
            participantId,
            language.Trim(),
            sourceCode);
    }

    public void MarkQueued()
    {
        Status = SubmissionStatus.Queued;
    }

    public void MarkRunning()
    {
        Status = SubmissionStatus.Running;
    }

    public void MarkAccepted(int totalTests)
    {
        Status = SubmissionStatus.Accepted;
        PassedTestCases = totalTests;
        TotalTestCases = totalTests;
    }

    public void MarkFailed(
        SubmissionStatus failureStatus,
        int passed,
        int total)
    {
        if (failureStatus == SubmissionStatus.Accepted)
            throw new ArgumentException("Invalid failure status.");

        Status = failureStatus;
        PassedTestCases = passed;
        TotalTestCases = total;
    }
}