namespace SolveIt.Api.Contracts;

public sealed class SubmitSolutionRequestDto
{
    public Guid TournamentId { get; set; }
    public Guid ProblemId { get; set; }
    public string Language { get; set; } = null!;
    public string SourceCode { get; set; } = null!;
}
