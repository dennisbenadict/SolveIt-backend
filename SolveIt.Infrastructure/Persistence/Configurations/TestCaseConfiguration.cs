using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolveIt.Domain.TestCases;
using SolveIt.Domain.TournamentProblems;

namespace SolveIt.Infrastructure.Persistence.Configurations;

internal sealed class TestCaseConfiguration
    : IEntityTypeConfiguration<TestCase>
{
    public void Configure(EntityTypeBuilder<TestCase> builder)
    {
        builder.ToTable("test_cases");

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
            .HasDatabaseName("IX_test_cases_problem");

        builder.HasOne<TournamentProblem>()
            .WithMany()
            .HasForeignKey(x => x.TournamentProblemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.Input)
            .HasColumnName("input")
            .HasColumnType("text")
            .IsRequired();

        builder.Property(x => x.ExpectedOutput)
            .HasColumnName("expected_output")
            .HasColumnType("text")
            .IsRequired();

        builder.Property(x => x.IsHidden)
            .HasColumnName("is_hidden")
            .IsRequired();

        builder.Property(x => x.OrderIndex)
            .HasColumnName("order_index")
            .IsRequired();

        builder.HasIndex(x => new { x.TournamentProblemId, x.OrderIndex })
            .IsUnique()
            .HasDatabaseName("IX_test_case_order");

        builder.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .HasColumnType("timestamp with time zone");

        builder.Property(x => x.RowVersion)
            .HasColumnName("row_version")
            .IsConcurrencyToken();
    }
}
