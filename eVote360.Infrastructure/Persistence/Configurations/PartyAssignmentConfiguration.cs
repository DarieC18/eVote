using EVote360.Domain.Entities.Assignments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EVote360.Infrastructure.Persistence.Configurations;

public class PartyAssignmentConfiguration : IEntityTypeConfiguration<PartyAssignment>
{
    public void Configure(EntityTypeBuilder<PartyAssignment> b)
    {
        b.ToTable("PartyAssignments");
        b.HasKey(x => x.Id);

        b.Property(x => x.Id).IsRequired();
        b.Property(x => x.PartyId).IsRequired();

        b.HasOne<Domain.Entities.Party.Party>()
            .WithMany()
            .HasForeignKey(x => x.PartyId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(x => x.Id).IsUnique();
        b.Property(x => x.CreatedAt).IsRequired();
    }
}
