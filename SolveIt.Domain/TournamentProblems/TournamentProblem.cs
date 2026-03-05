namespace SolveIt.Domain.TournamentProblems;

public sealed class TournamentProblem
{
    public Guid Id { get; private set; }

    public Guid TournamentId { get; private set; }

    public string Title { get; private set; } = null!;

    public string Description { get; private set; } = null!;

    public int TimeLimitMs { get; private set; }

    public int MemoryLimitMb { get; private set; }

    public int OrderIndex { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public byte[] RowVersion { get; private set; } = null!;

    private TournamentProblem() { }

    private TournamentProblem(
        Guid id,
        Guid tournamentId,
        string title,
        string description,
        int timeLimitMs,
        int memoryLimitMb,
        int orderIndex)
    {
        Id = id;
        TournamentId = tournamentId;
        Title = title;
        Description = description;
        TimeLimitMs = timeLimitMs;
        MemoryLimitMb = memoryLimitMb;
        OrderIndex = orderIndex;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public static TournamentProblem Create(
        Guid tournamentId,
        string title,
        string description,
        int timeLimitMs,
        int memoryLimitMb,
        int orderIndex)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title required.");

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description required.");

        if (timeLimitMs <= 0)
            throw new ArgumentException("Invalid time limit.");

        if (memoryLimitMb <= 0)
            throw new ArgumentException("Invalid memory limit.");

        return new TournamentProblem(
            Guid.NewGuid(),
            tournamentId,
            title.Trim(),
            description.Trim(),
            timeLimitMs,
            memoryLimitMb,
            orderIndex);
    }
}
