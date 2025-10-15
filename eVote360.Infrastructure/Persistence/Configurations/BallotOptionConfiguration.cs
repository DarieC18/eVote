using EVote360.Domain.Entities.Ballot;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EVote360.Infrastructure.Persistence.Configurations;

public class BallotOptionConfiguration : IEntityTypeConfiguration<BallotOption>
{
    public void Configure(EntityTypeBuilder<BallotOption> b)
    {
        b.ToTable("BallotOptions");
        b.HasKey(x => x.Id);

        b.Property(x => x.ElectionBallotId).IsRequired();
        b.HasOne<ElectionBallot>()
            .WithMany()
            .HasForeignKey(x => x.ElectionBallotId)
            .OnDelete(DeleteBehavior.Cascade);

        // Candidate/Party pueden ser null (para "Ninguno")
        b.Property(x => x.CandidateId).IsRequired(false);
        b.Property(x => x.PartyId).IsRequired(false);

        // Evitar duplicados de opciones dentro del mismo ballot
        b.HasIndex(x => new { x.ElectionBallotId, x.CandidateId, x.PartyId, x.IsNinguno }).IsUnique();

        b.Property(x => x.IsNinguno).IsRequired();
        b.Property(x => x.CreatedAt).IsRequired();
    }
}
