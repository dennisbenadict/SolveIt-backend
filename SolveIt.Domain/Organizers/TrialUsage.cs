namespace SolveIt.Domain.Organizers;

public sealed class TrialUsage
{
    public Guid Id { get; private set; }
    public Guid OrganizerId { get; private set; }
    public string? DeviceFingerprint { get; private set; }
    public string? IpAddress { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private TrialUsage() { }

    private TrialUsage(
        Guid id,
        Guid organizerId,
        string? deviceFingerprint,
        string? ipAddress)
    {
        Id = id;
        OrganizerId = organizerId;
        DeviceFingerprint = deviceFingerprint;
        IpAddress = ipAddress;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public static TrialUsage Create(
        Guid organizerId,
        string? deviceFingerprint,
        string? ipAddress)
    {
        return new TrialUsage(
            Guid.NewGuid(),
            organizerId,
            deviceFingerprint,
            ipAddress);
    }
}