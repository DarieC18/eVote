using EVote360.Domain.Entities.Election;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EVote360.Infrastructure.Persistence.Configurations;

public class ElectionConfiguration : IEntityTypeConfiguration<Election>
{
    public void Configure(EntityTypeBuilder<Election> b)
    {
        b.ToTable("Elections");
        b.HasKey(x => x.Id);

        b.Property(x => x.Name).HasMaxLength(150).IsRequired();
        b.Property(x => x.ScheduledAt).IsRequired();
        b.Property(x => x.State).IsRequired();

        b.Property(x => x.CreatedAt).IsRequired();
    }
}
