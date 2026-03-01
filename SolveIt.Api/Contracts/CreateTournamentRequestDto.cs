namespace SolveIt.Api.Contracts;

public sealed class CreateTournamentRequestDto
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string? DeviceFingerprint { get; set; }
    public DateTime StartTimeUtc { get; set; }
    public DateTime EndTimeUtc { get; set; }
}
