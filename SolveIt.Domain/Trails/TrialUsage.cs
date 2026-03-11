public sealed class TrialUsage
{
    public Guid Id { get; private set; }

    public Guid OrganizerId { get; private set; }

    public string DeviceFingerprint { get; private set; } = null!;

    public string IpAddress { get; private set; } = null!;

    public DateTime UsedAtUtc { get; private set; }

    private TrialUsage() { }

    private TrialUsage(
        Guid organizerId,
        string fingerprint,
        string ip)
    {
        Id = Guid.NewGuid();
        OrganizerId = organizerId;
        DeviceFingerprint = fingerprint;
        IpAddress = ip;
        UsedAtUtc = DateTime.UtcNow;
    }

    public static TrialUsage Create(
        Guid organizerId,
        string fingerprint,
        string ip)
    {
        if (string.IsNullOrWhiteSpace(fingerprint))
            throw new ArgumentException("Fingerprint required");

        if (string.IsNullOrWhiteSpace(ip))
            throw new ArgumentException("IP required");

        return new TrialUsage(
            organizerId,
            fingerprint,
            ip);
    }
}
