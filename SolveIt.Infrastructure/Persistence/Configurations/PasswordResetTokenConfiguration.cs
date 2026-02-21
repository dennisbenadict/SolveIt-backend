using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolveIt.Domain.Organizers;

namespace SolveIt.Infrastructure.Persistence.Configurations;

public sealed class PasswordResetTokenConfiguration
    : IEntityTypeConfiguration<PasswordResetToken>
{
    public void Configure(EntityTypeBuilder<PasswordResetToken> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.TokenHash)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(x => x.ExpiresAtUtc)
            .IsRequired();

        builder.Property(x => x.IsUsed)
            .IsRequired();

        // Unique token hash
        builder.HasIndex(x => x.TokenHash)
            .IsUnique();

        builder.HasIndex(x => x.OrganizerId);
    }
}
