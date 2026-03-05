using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolveIt.Domain.Submissions;

namespace SolveIt.Infrastructure.Persistence.Configurations;

internal sealed class SubmissionConfiguration
    : IEntityTypeConfiguration<Submission>
{
    public void Configure(EntityTypeBuilder<Submission> builder)
    {
        builder.ToTable("submissions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever();

        builder.Property(x => x.TournamentProblemId)
            .HasColumnName("problem_id")
            .HasColumnType("uuid")
            .IsRequired();

        builder.HasIndex(x => x.TournamentProblemId)
            .HasDatabaseName("IX_submissions_problem");

        builder.Property(x => x.ParticipantId)
            .HasColumnName("participant_id")
            .HasColumnType("uuid")
            .IsRequired();

        builder.HasIndex(x => x.ParticipantId)
            .HasDatabaseName("IX_submissions_participant");

        builder.Property(x => x.Language)
            .HasColumnName("language")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.SourceCode)
            .HasColumnName("source_code")
            .HasColumnType("text")
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .IsRequired();

        builder.Property(x => x.PassedTestCases)
            .HasColumnName("passed_test_cases");

        builder.Property(x => x.TotalTestCases)
            .HasColumnName("total_test_cases");

        builder.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .HasColumnType("timestamp with time zone");

        builder.Property(x => x.RowVersion)
            .HasColumnName("row_version")
            .IsConcurrencyToken();
    }
}
