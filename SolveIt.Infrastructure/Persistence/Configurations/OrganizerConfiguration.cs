using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Solvelt.Domain.Organizers;

namespace Solvelt.Infrastructure.Persistence.Configurations
{
    internal sealed class OrganizerConfiguration : IEntityTypeConfiguration<Organizer>
    {
        public void Configure(EntityTypeBuilder<Organizer> builder)
        {
            // Table
            builder.ToTable("organizers");

            // Primary Key
            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id)
                .HasColumnName("id")
                .HasColumnType("uuid")
                .ValueGeneratedNever();

            // Email (unique)
            builder.Property(o => o.Email)
                .HasColumnName("email")
                .HasMaxLength(255)
                .IsRequired();

            builder.HasIndex(o => o.Email)
                .IsUnique();

            // Name
            builder.Property(o => o.Name)
                .HasColumnName("name")
                .HasMaxLength(150)
                .IsRequired();

            // Auth Provider
            builder.Property(o => o.AuthProvider)
                .HasColumnName("auth_provider")
                .HasMaxLength(50)
                .IsRequired();

            // Created At
            builder.Property(o => o.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp with time zone")
                .IsRequired();
        }
    }
}

