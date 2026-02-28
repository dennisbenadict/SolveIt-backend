using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolveIt.Domain.Organizers;

namespace SolveIt.Infrastructure.Persistence.Configurations;

internal sealed class TrialUsageConfiguration
    : IEntityTypeConfiguration<TrialUsage>
{
    public void Configure(EntityTypeBuilder<TrialUsage> builder)
    {
        // Table
        builder.ToTable("trial_usages");

        // Primary Key
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever();

        // Organizer Id
        builder.Property(t => t.OrganizerId)
            .HasColumnName("organizer_id")
            .HasColumnType("uuid")
            .IsRequired();

        builder.HasIndex(t => t.OrganizerId)
            .HasDatabaseName("IX_trial_usages_organizer_id");

        // Device Fingerprint
        builder.Property(t => t.DeviceFingerprint)
            .HasColumnName("device_fingerprint")
            .HasColumnType("text")
            .IsRequired(false);

        builder.HasIndex(t => t.DeviceFingerprint)
            .HasDatabaseName("IX_trial_usages_device_fingerprint");

        // IP Address
        builder.Property(t => t.IpAddress)
            .HasColumnName("ip_address")
            .HasColumnType("varchar(64)")
            .IsRequired(false);

        builder.HasIndex(t => t.IpAddress)
            .HasDatabaseName("IX_trial_usages_ip_address");

        // Created At
        builder.Property(t => t.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .HasColumnType("timestamp with time zone")
            .IsRequired();
    }
}
