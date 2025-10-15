using EVote360.Domain.Base;
using EVote360.Domain.Entities.Alliance;
using EVote360.Domain.Entities.Assignments;
using EVote360.Domain.Entities.Ballot;
using EVote360.Domain.Entities.Candidate;
using EVote360.Domain.Entities.Citizen;
using EVote360.Domain.Entities.Election;
using EVote360.Domain.Entities.Party;
using EVote360.Domain.Entities.Position;
using EVote360.Domain.Entities.Vote;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace EVote360.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // DbSets
    public DbSet<Position> Positions => Set<Position>();
    public DbSet<Party> Parties => Set<Party>();
    public DbSet<Citizen> Citizens => Set<Citizen>();
    public DbSet<Candidate> Candidates => Set<Candidate>();
    public DbSet<Alliance> Alliances => Set<Alliance>();
    public DbSet<PartyAssignment> PartyAssignments => Set<PartyAssignment>();
    public DbSet<Election> Elections => Set<Election>();
    public DbSet<ElectionBallot> ElectionBallots => Set<ElectionBallot>();
    public DbSet<BallotOption> BallotOptions => Set<BallotOption>();
    public DbSet<Vote> Votes => Set<Vote>();
    public DbSet<VoteItem> VoteItems => Set<VoteItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Filtro global para ISoftDeletable
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(AppDbContext).GetMethod(nameof(ApplyIsActiveFilter),
                    BindingFlags.NonPublic | BindingFlags.Static)!
                    .MakeGenericMethod(entityType.ClrType);
                method.Invoke(null, new object[] { modelBuilder });
            }
        }

        base.OnModelCreating(modelBuilder);
    }

    private static void ApplyIsActiveFilter<TEntity>(ModelBuilder builder) where TEntity : class, ISoftDeletable
    {
        builder.Entity<TEntity>().HasQueryFilter(x => x.IsActive);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var utcNow = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Property(nameof(AuditableEntity.CreatedAt)).CurrentValue = utcNow;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Property(nameof(AuditableEntity.UpdatedAt)).CurrentValue = utcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
