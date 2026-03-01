using Microsoft.EntityFrameworkCore;
//using SolveIt.Domain.Submissions;
using SolveIt.Domain.Tournaments;
using SolveIt.Domain.Organizers;
using SolveIt.Domain.Participants;

namespace SolveIt.Infrastructure.Persistence
{
    public sealed class SolveItDbContext : DbContext
    {
        public SolveItDbContext(DbContextOptions<SolveItDbContext> options)
            : base(options)
        {
        }

        // DbSets (one per aggregate root)
        public DbSet<Organizer> Organizers => Set<Organizer>();
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
        public DbSet<Participant> Participants { get; set; } = null!;
        public DbSet<TrialUsage> TrialUsages { get; set; }
        public DbSet<Tournament> Tournaments { get; set; }
        //public DbSet<Submission> Submissions => Set<Submission>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(SolveItDbContext).Assembly
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}

