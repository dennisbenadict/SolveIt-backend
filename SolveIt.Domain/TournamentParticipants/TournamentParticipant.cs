namespace SolveIt.Domain.TournamentParticipants;

public sealed class TournamentParticipant
{
    public Guid Id { get; private set; }

    public Guid TournamentId { get; private set; }

    public Guid ParticipantId { get; private set; }

    public DateTime JoinedAtUtc { get; private set; }

    public byte[] RowVersion { get; private set; } = null!;

    private TournamentParticipant() { }

    private TournamentParticipant(
        Guid id,
        Guid tournamentId,
        Guid participantId)
    {
        Id = id;
        TournamentId = tournamentId;
        ParticipantId = participantId;
        JoinedAtUtc = DateTime.UtcNow;
    }

    public static TournamentParticipant Create(
        Guid tournamentId,
        Guid participantId)
    {
        return new TournamentParticipant(
            Guid.NewGuid(),
            tournamentId,
            participantId);
    }
}
