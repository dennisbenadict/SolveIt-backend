using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolveIt.Domain.Organizers;
using SolveIt.Domain.Tournaments;

namespace SolveIt.Infrastructure.Persistence.Configurations;

internal sealed class TournamentConfiguration
    : IEntityTypeConfiguration<Tournament>
{
    public void Configure(EntityTypeBuilder<Tournament> builder)
    {
        // Table
        builder.ToTable("tournaments");

        // Primary Key
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever();

        // Organizer FK
        builder.Property(t => t.OrganizerId)
            .HasColumnName("organizer_id")
            .HasColumnType("uuid")
            .IsRequired();

        builder.HasOne<Organizer>()
            .WithMany()
            .HasForeignKey(t => t.OrganizerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(t => t.OrganizerId)
            .HasDatabaseName("IX_tournaments_organizer_id");

        // Title
        builder.Property(t => t.Title)
            .HasColumnName("title")
            .HasMaxLength(200)
            .IsRequired();

        // Description
        builder.Property(t => t.Description)
            .HasColumnName("description")
            .HasColumnType("text")
            .IsRequired(false);

        // Start Time
        builder.Property(t => t.StartTimeUtc)
            .HasColumnName("start_time_utc")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        // End Time
        builder.Property(t => t.EndTimeUtc)
            .HasColumnName("end_time_utc")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        // Status
        builder.Property(t => t.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .IsRequired();

        builder.HasIndex(t => t.Status)
            .HasDatabaseName("IX_tournaments_status");

        // Free Trial Flag
        builder.Property(t => t.IsFreeTrial)
            .HasColumnName("is_free_trial")
            .IsRequired();

        // Created At
        builder.Property(t => t.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        // Composite index for dashboard queries
        builder.HasIndex(t => new { t.OrganizerId, t.CreatedAtUtc })
            .HasDatabaseName("IX_tournaments_organizer_created");

        // Concurrency (PostgreSQL safe)
        builder.Property(t => t.RowVersion)
            .IsConcurrencyToken()
            .HasColumnName("row_version");
    }
}
