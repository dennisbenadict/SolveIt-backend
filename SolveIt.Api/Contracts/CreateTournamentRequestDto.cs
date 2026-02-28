namespace SolveIt.Api.Contracts;

public sealed class CreateTournamentRequestDto
{
    public string Title { get; set; } = null!;
    public string? DeviceFingerprint { get; set; }
}
