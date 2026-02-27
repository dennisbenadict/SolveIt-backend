using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolveIt.Domain.Participants;

namespace SolveIt.Infrastructure.Persistence.Configurations
{
    internal sealed class ParticipantConfiguration
        : IEntityTypeConfiguration<Participant>
    {
        public void Configure(EntityTypeBuilder<Participant> builder)
        {
            // Table
            builder.ToTable("participants");

            // Primary Key
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .HasColumnName("id")
                .HasColumnType("uuid")
                .ValueGeneratedNever();

            // Email (unique)
            builder.Property(p => p.Email)
                .HasColumnName("email")
                .HasMaxLength(254)
                .IsRequired();

            builder.HasIndex(p => p.Email)
                .IsUnique();

            // Phone (unique)
            builder.Property(p => p.PhoneNumber)
                .HasColumnName("phone_number")
                .HasMaxLength(13)
                .IsRequired();

            builder.HasIndex(p => p.PhoneNumber)
                .IsUnique();

            // Name
            builder.Property(p => p.Name)
                .HasColumnName("name")
                .HasMaxLength(150)
                .IsRequired();

            // Password Hash
            builder.Property(p => p.PasswordHash)
                .HasColumnName("password_hash")
                .HasMaxLength(200)
                .IsRequired();

            // Created At
            builder.Property(p => p.CreatedAtUtc)
                .HasColumnName("created_at_utc")
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            // Lockout Fields
            builder.Property(p => p.FailedLoginAttempts)
                .HasColumnName("failed_login_attempts")
                .IsRequired();

            builder.Property(p => p.LockoutEndUtc)
                .HasColumnName("lockout_end_utc")
                .HasColumnType("timestamp with time zone")
                .IsRequired(false);

            // Is Blocked
            builder.Property(p => p.IsBlocked)
                .HasColumnName("is_blocked")
                .HasColumnType("boolean")
                .IsRequired()
                .HasDefaultValue(false);

            // Concurrency Token
            builder.Property(p => p.RowVersion)
                .HasColumnName("row_version")
                .IsRowVersion()
                .IsConcurrencyToken();

            // User Role
            builder.Property(p => p.Role)
                .HasColumnName("role")
                .HasConversion<int>()
                .IsRequired();
        }
    }
}