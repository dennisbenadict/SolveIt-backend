using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolveIt.Domain.Participants;
using SolveIt.Domain.TournamentParticipants;
using SolveIt.Domain.Tournaments;

namespace SolveIt.Infrastructure.Persistence.Configurations;

internal sealed class TournamentParticipantConfiguration
    : IEntityTypeConfiguration<TournamentParticipant>
{
    public void Configure(EntityTypeBuilder<TournamentParticipant> builder)
    {
        builder.ToTable("tournament_participants");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever();

        builder.Property(x => x.TournamentId)
            .HasColumnName("tournament_id")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(x => x.ParticipantId)
            .HasColumnName("participant_id")
            .HasColumnType("uuid")
            .IsRequired();

        builder.HasIndex(x => new { x.TournamentId, x.ParticipantId })
            .IsUnique()
            .HasDatabaseName("IX_tournament_participants_unique");

        builder.HasOne<Tournament>()
            .WithMany()
            .HasForeignKey(x => x.TournamentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Participant>()
            .WithMany()
            .HasForeignKey(x => x.ParticipantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.JoinedAtUtc)
            .HasColumnName("joined_at_utc")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(x => x.RowVersion)
            .IsConcurrencyToken()
            .HasColumnName("row_version");
    }
}
