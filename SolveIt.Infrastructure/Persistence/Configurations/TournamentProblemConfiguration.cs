using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolveIt.Domain.TournamentProblems;
using SolveIt.Domain.Tournaments;

namespace SolveIt.Infrastructure.Persistence.Configurations;

internal sealed class TournamentProblemConfiguration
    : IEntityTypeConfiguration<TournamentProblem>
{
    public void Configure(EntityTypeBuilder<TournamentProblem> builder)
    {
        builder.ToTable("tournament_problems");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever();

        builder.Property(x => x.TournamentId)
            .HasColumnName("tournament_id")
            .HasColumnType("uuid")
            .IsRequired();

        builder.HasIndex(x => x.TournamentId)
            .HasDatabaseName("IX_tournament_problems_tournament_id");

        builder.HasOne<Tournament>()
            .WithMany()
            .HasForeignKey(x => x.TournamentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.Title)
            .HasColumnName("title")
            .HasColumnType("varchar(200)")
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasColumnType("text")
            .IsRequired();

        builder.Property(x => x.TimeLimitMs)
            .HasColumnName("time_limit_ms")
            .IsRequired();

        builder.Property(x => x.MemoryLimitMb)
            .HasColumnName("memory_limit_mb")
            .IsRequired();

        builder.Property(x => x.OrderIndex)
            .HasColumnName("order_index")
            .IsRequired();

        builder.HasIndex(x => new { x.TournamentId, x.OrderIndex })
            .IsUnique()
            .HasDatabaseName("IX_tournament_problem_order");

        builder.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .HasColumnType("timestamp with time zone");

        builder.Property(x => x.RowVersion)
            .IsConcurrencyToken()
            .HasColumnName("row_version");
    }
}
