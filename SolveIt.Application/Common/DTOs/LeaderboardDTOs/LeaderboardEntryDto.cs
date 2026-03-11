namespace SolveIt.Application.Common.DTOs.LeaderboardDTOs;


public sealed class LeaderboardEntryDto
{
    public Guid ParticipantId { get; set; }
    public int SolvedProblems { get; set; }
    public DateTime LastAcceptedAtUtc { get; set; }
    public int Rank { get; set; }
}
