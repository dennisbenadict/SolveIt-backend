using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolveIt.Domain.Organizers;

namespace SolveIt.Infrastructure.Persistence.Configurations;

internal sealed class RefreshTokenConfiguration
    : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        // Table
        builder.ToTable("refresh_tokens");

        // Primary Key
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever();

        // User Id (organizer or participant)
        builder.Property(r => r.UserId)
            .HasColumnName("organizer_id")
            .HasColumnType("uuid")
            .IsRequired();

        builder.HasIndex(r => r.UserId)
            .HasDatabaseName("IX_refresh_tokens_organizer_id");

        // Token Hash (SHA-256 hex = 64 chars)
        builder.Property(r => r.TokenHash)
            .HasColumnName("token_hash")
            .HasMaxLength(64)
            .IsRequired();

        builder.HasIndex(r => r.TokenHash)
            .IsUnique()
            .HasDatabaseName("IX_refresh_tokens_token_hash");

        // Created At
        builder.Property(r => r.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        // Expiry
        builder.Property(r => r.ExpiresAtUtc)
            .HasColumnName("expires_at_utc")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.HasIndex(r => r.ExpiresAtUtc)
            .HasDatabaseName("IX_refresh_tokens_expires_at_utc");

        // Revoked flag
        builder.Property(r => r.IsRevoked)
            .HasColumnName("is_revoked")
            .IsRequired();

        // Revoked At (nullable)
        builder.Property(r => r.RevokedAtUtc)
            .HasColumnName("revoked_at_utc")
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);

        // Replaced By Token (nullable self-reference)
        builder.Property(r => r.ReplacedByTokenId)
            .HasColumnName("replaced_by_token_id")
            .HasColumnType("uuid")
            .IsRequired(false);

        builder.HasIndex(r => r.ReplacedByTokenId)
            .HasDatabaseName("IX_refresh_tokens_replaced_by_token_id");

        builder.HasOne<RefreshToken>()
            .WithOne()
            .HasForeignKey<RefreshToken>(r => r.ReplacedByTokenId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
