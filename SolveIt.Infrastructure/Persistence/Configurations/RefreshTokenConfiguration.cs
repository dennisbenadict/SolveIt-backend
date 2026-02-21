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

        // Organizer FK
        builder.Property(r => r.OrganizerId)
            .HasColumnName("organizer_id")
            .HasColumnType("uuid")
            .IsRequired();

        builder.HasIndex(r => r.OrganizerId)
            .HasDatabaseName("IX_refresh_tokens_organizer_id");

        builder.HasOne<Organizer>()
            .WithMany()
            .HasForeignKey(r => r.OrganizerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Token Hash (SHA-256 hex = 64 chars)
        builder.Property(r => r.TokenHash)
            .HasColumnName("token_hash")
            .HasMaxLength(64)
            .IsRequired();

        builder.HasIndex(r => r.TokenHash)
            .IsUnique()
            .HasDatabaseName("IX_refresh_tokens_token_hash");

        // Expiry
        builder.Property(r => r.ExpiresAtUtc)
            .HasColumnName("expires_at_utc")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        // Created At
        builder.Property(r => r.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        // Revoked flag
        builder.Property(r => r.IsRevoked)
            .HasColumnName("is_revoked")
            .IsRequired();
    }
}
