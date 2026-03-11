using SolveIt.Domain.Submissions;

namespace SolveIt.Application.Common.DTOs.FinalSubmissionDTOs;

public sealed class SubmissionStatusDto
{
    public Guid Id { get; set; }
    public SubmissionStatus Status { get; set; }
    public int PassedTestCases { get; set; }
    public int TotalTestCases { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public Guid TournamentProblemId { get; set; }
}
