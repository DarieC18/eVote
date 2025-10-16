using EVote360.Domain.Entities.Party;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EVote360.Infrastructure.Persistence.Configurations;

public sealed class PartyConfiguration : IEntityTypeConfiguration<Party>
{
    public void Configure(EntityTypeBuilder<Party> b)
    {
        b.ToTable("Parties");
        b.HasKey(x => x.Id);

        b.Property(x => x.Name).IsRequired().HasMaxLength(200);
        b.Property(x => x.Siglas).IsRequired().HasMaxLength(12);
        b.Property(x => x.Description).HasMaxLength(400);
        b.Property(x => x.LogoPath).HasMaxLength(260);


        b.HasIndex(x => x.Siglas).IsUnique();
    }
}
