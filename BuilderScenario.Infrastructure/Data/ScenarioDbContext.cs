using BuilderScenario.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace BuilderScenario.Infrastructure.Data
{
    public class ScenarioDbContext : DbContext
    {
        public DbSet<Scenario> Scenarios => Set<Scenario>();
        public DbSet<ActionGroup> ActionGroups => Set<ActionGroup>();
        public DbSet<StepItem> Steps => Set<StepItem>();
        public DbSet<ActionItem> Actions => Set<ActionItem>();

        public ScenarioDbContext(DbContextOptions<ScenarioDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Scenario>()
                .HasMany(s => s.Groups)
                .WithOne()
                .HasForeignKey(g => g.ScenarioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ActionGroup>()
                .HasMany(g => g.Steps)
                .WithOne()
                .HasForeignKey(s => s.ActionGroupId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StepItem>()
                .HasMany(s => s.Actions)
                .WithOne()
                .HasForeignKey(a => a.StepItemId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
