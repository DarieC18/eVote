using EVote360.Domain.Base;
using EVote360.Domain.Entities;
using EVote360.Domain.Entities.Alliance;
using EVote360.Domain.Entities.Assignments;
using EVote360.Domain.Entities.Ballot;
using EVote360.Domain.Entities.Candidate;
using EVote360.Domain.Entities.Citizen;
using EVote360.Domain.Entities.Election;
using EVote360.Domain.Entities.Vote;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

using PartyEntity = EVote360.Domain.Entities.Party.Party;
using PositionEntity = EVote360.Domain.Entities.Position.Position;

namespace EVote360.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // DbSets
    public DbSet<PositionEntity> Positions => Set<PositionEntity>();
    public DbSet<PartyEntity> Parties => Set<PartyEntity>();
    public DbSet<Citizen> Citizens => Set<Citizen>();
    public DbSet<Candidate> Candidates => Set<Candidate>();
    public DbSet<Alliance> Alliances => Set<Alliance>();
    public DbSet<PartyAssignment> PartyAssignments => Set<PartyAssignment>();
    public DbSet<Election> Elections => Set<Election>();
    public DbSet<ElectionBallot> ElectionBallots => Set<ElectionBallot>();
    public DbSet<BallotOption> BallotOptions => Set<BallotOption>();
    public DbSet<Vote> Votes => Set<Vote>();
    public DbSet<VoteItem> VoteItems => Set<VoteItem>();
    public DbSet<Candidatura> Candidaturas => Set<Candidatura>();
    public DbSet<EVote360.Domain.Entities.Usuario> Usuarios { get; set; } = null!;


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Filtro global ISoftDeletable
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

        modelBuilder.Entity<Candidatura>(e =>
        {
            e.HasIndex(x => new { x.PartyId, x.PositionId }).IsUnique();
            e.HasIndex(x => new { x.PartyId, x.CandidateId }).IsUnique();
        });

        modelBuilder.Entity<EVote360.Domain.Entities.Assignments.PartyAssignment>(b =>
        {
            b.HasKey(pa => pa.Id);

            b.Property(pa => pa.Activo)
             .IsRequired();

            b.HasOne(pa => pa.Usuario)
             .WithMany(u => u.PartyAssignments)
             .HasForeignKey(pa => pa.UsuarioId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(pa => pa.Party)
             .WithMany()
             .HasForeignKey(pa => pa.PartyId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Usuario>(b =>
        {
            b.HasKey(u => u.Id);

            b.Property(u => u.Nombre).IsRequired().HasMaxLength(80);
            b.Property(u => u.Apellido).IsRequired().HasMaxLength(80);
            b.Property(u => u.Email).IsRequired().HasMaxLength(120);
            b.Property(u => u.NombreUsuario).IsRequired().HasMaxLength(50);
            b.Property(u => u.Rol).IsRequired().HasMaxLength(30);
            b.Property(u => u.PasswordHash).IsRequired();

            b.HasMany(u => u.PartyAssignments)
             .WithOne(pa => pa.Usuario)
             .HasForeignKey(pa => pa.UsuarioId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<EVote360.Domain.Entities.Usuario>(b =>
        {
            b.HasIndex(x => x.Email).IsUnique();
            b.HasIndex(x => x.NombreUsuario).IsUnique();
            b.Property(x => x.Nombre).HasMaxLength(60).IsRequired();
            b.Property(x => x.Apellido).HasMaxLength(60).IsRequired();
            b.Property(x => x.Email).HasMaxLength(120).IsRequired();
            b.Property(x => x.NombreUsuario).HasMaxLength(40).IsRequired();
            b.Property(x => x.Rol).HasMaxLength(20).IsRequired();
        });

        modelBuilder.Entity<EVote360.Domain.Entities.Assignments.PartyAssignment>(b =>
        {
            b.HasKey(pa => pa.Id);

            b.HasOne(pa => pa.Usuario)
             .WithMany(u => u.PartyAssignments)
             .HasForeignKey(pa => pa.UsuarioId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(pa => pa.Party)
             .WithMany()
             .HasForeignKey(pa => pa.PartyId)
             .OnDelete(DeleteBehavior.Cascade);
        });

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
                entry.Property(nameof(AuditableEntity.CreatedAt)).CurrentValue = utcNow;

            if (entry.State == EntityState.Modified)
                entry.Property(nameof(AuditableEntity.UpdatedAt)).CurrentValue = utcNow;
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}
