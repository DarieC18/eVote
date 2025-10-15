using EVote360.Domain.Entities.Position;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EVote360.Infrastructure.Persistence.Configurations;

public class PositionConfiguration : IEntityTypeConfiguration<Position>
{
    public void Configure(EntityTypeBuilder<Position> b)
    {
        b.ToTable("Positions");
        b.HasKey(x => x.Id);

        b.Property(x => x.Name)
            .HasMaxLength(120)
            .IsRequired();

        b.HasIndex(x => x.Name).IsUnique();

        b.Property(x => x.IsActive).IsRequired();
        b.Property(x => x.CreatedAt).IsRequired();
    }
}
