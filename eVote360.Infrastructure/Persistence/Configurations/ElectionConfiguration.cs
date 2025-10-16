using EVote360.Domain.Entities.Election;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EVote360.Infrastructure.Persistence.Configurations;

public sealed class ElectionConfiguration : IEntityTypeConfiguration<Election>
{
    public void Configure(EntityTypeBuilder<Election> b)
    {
        b.ToTable("Elections");
        b.HasKey(x => x.Id);

        b.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        b.Property(x => x.Year)
            .IsRequired();

        b.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        b.Property(x => x.StartedAt)
            .IsRequired(false);

        b.Property(x => x.FinishedAt)
            .IsRequired(false);

        b.HasIndex(x => new { x.Year, x.Name }).IsUnique();
    }
}
