using EVote360.Domain.Entities.Candidate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EVote360.Infrastructure.Persistence.Configurations;

public class CandidateConfiguration : IEntityTypeConfiguration<Candidate>
{
    public void Configure(EntityTypeBuilder<Candidate> b)
    {
        b.ToTable("Candidates");
        b.HasKey(x => x.Id);

        b.Property(x => x.FirstName).HasMaxLength(120).IsRequired();
        b.Property(x => x.LastName).HasMaxLength(120).IsRequired();

        b.Property(x => x.PartyId).IsRequired();
        b.HasOne<Domain.Entities.Party.Party>()
            .WithMany()
            .HasForeignKey(x => x.PartyId)
            .OnDelete(DeleteBehavior.Restrict);

        b.Property(x => x.IsActive).IsRequired();
        b.Property(x => x.CreatedAt).IsRequired();
    }
}
