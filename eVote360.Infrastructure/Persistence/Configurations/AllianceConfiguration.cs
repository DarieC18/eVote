using EVote360.Domain.Entities.Alliance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EVote360.Infrastructure.Persistence.Configurations;

public class AllianceConfiguration : IEntityTypeConfiguration<Alliance>
{
    public void Configure(EntityTypeBuilder<Alliance> b)
    {
        b.ToTable("Alliances");
        b.HasKey(x => x.Id);

        b.Property(x => x.PartyAId).IsRequired();
        b.Property(x => x.PartyBId).IsRequired();

        b.HasOne<Domain.Entities.Party.Party>()
            .WithMany()
            .HasForeignKey(x => x.PartyAId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne<Domain.Entities.Party.Party>()
            .WithMany()
            .HasForeignKey(x => x.PartyBId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(x => new { x.PartyAId, x.PartyBId }).IsUnique();

        // No permitir A==B
        b.HasCheckConstraint("CK_Alliance_DifferentParties", "[PartyAId] <> [PartyBId]");

        b.Property(x => x.Status).IsRequired();
        b.Property(x => x.IsActive).IsRequired();
        b.Property(x => x.CreatedAt).IsRequired();
    }
}
