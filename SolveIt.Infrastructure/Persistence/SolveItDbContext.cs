using Microsoft.EntityFrameworkCore;
//using SolveIt.Domain.Participants;
//using SolveIt.Domain.Submissions;
//using SolveIt.Domain.Tournaments;
using Solvelt.Domain.Organizers;

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
        //public DbSet<Participant> Participants => Set<Participant>();
        //public DbSet<Tournament> Tournaments => Set<Tournament>();
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

