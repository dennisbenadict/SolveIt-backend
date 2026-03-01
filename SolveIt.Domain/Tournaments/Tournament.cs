using SolveIt.Domain.Exceptions;

namespace SolveIt.Domain.Tournaments;

public sealed class Tournament
{
    public Guid Id { get; private set; }
    public Guid OrganizerId { get; private set; }

    public string Title { get; private set; } = null!;
    public string? Description { get; private set; }

    public DateTime StartTimeUtc { get; private set; }
    public DateTime EndTimeUtc { get; private set; }

    public TournamentStatus Status { get; private set; }

    public bool IsFreeTrial { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public byte[] RowVersion { get; private set; } = null!;

    private Tournament() { }

    private Tournament(
        Guid id,
        Guid organizerId,
        string title,
        string? description,
        DateTime startTimeUtc,
        DateTime endTimeUtc,
        bool isFreeTrial)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new TournamentTitleRequiredException();

        if (startTimeUtc >= endTimeUtc)
            throw new TournamentInvalidTimeRangeException();

        Id = id;
        OrganizerId = organizerId;
        Title = title.Trim();
        Description = description?.Trim();
        StartTimeUtc = startTimeUtc;
        EndTimeUtc = endTimeUtc;
        IsFreeTrial = isFreeTrial;

        Status = TournamentStatus.Draft;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public static Tournament Create(
        Guid organizerId,
        string title,
        string? description,
        DateTime startTimeUtc,
        DateTime endTimeUtc,
        bool isFreeTrial)
    {
        return new Tournament(
            Guid.NewGuid(),
            organizerId,
            title,
            description,
            startTimeUtc,
            endTimeUtc,
            isFreeTrial);
    }

    public void Publish()
    {
        if (Status != TournamentStatus.Draft)
            throw new TournamentNotDraftException();

        Status = TournamentStatus.Published;
    }

    public void MarkOngoing()
    {
        if (Status != TournamentStatus.Published)
            throw new TournamentNotPublishedException();

        Status = TournamentStatus.Ongoing;
    }

    public void Complete()
    {
        if (Status != TournamentStatus.Ongoing)
            throw new TournamentNotOngoingException();

        Status = TournamentStatus.Completed;
    }

    public void Cancel()
    {
        if (Status == TournamentStatus.Completed)
            throw new TournamentAlreadyCompletedException();

        Status = TournamentStatus.Cancelled;
    }
}
