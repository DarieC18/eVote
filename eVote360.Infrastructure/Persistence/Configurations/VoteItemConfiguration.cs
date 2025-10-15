using EVote360.Domain.Entities.Vote;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EVote360.Infrastructure.Persistence.Configurations;

public class VoteItemConfiguration : IEntityTypeConfiguration<VoteItem>
{
    public void Configure(EntityTypeBuilder<VoteItem> b)
    {
        b.ToTable("VoteItems");
        b.HasKey(x => x.Id);

        b.Property(x => x.VoteId).IsRequired();
        b.Property(x => x.ElectionBallotId).IsRequired();
        b.Property(x => x.BallotOptionId).IsRequired();

        b.HasOne<Domain.Entities.Ballot.ElectionBallot>()
            .WithMany()
            .HasForeignKey(x => x.ElectionBallotId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne<Domain.Entities.Ballot.BallotOption>()
            .WithMany()
            .HasForeignKey(x => x.BallotOptionId)
            .OnDelete(DeleteBehavior.Restrict);

        // Evitar repetir misma selección dentro de un voto
        b.HasIndex(x => new { x.VoteId, x.ElectionBallotId }).IsUnique();

        b.Property(x => x.CreatedAt).IsRequired();
    }
}
