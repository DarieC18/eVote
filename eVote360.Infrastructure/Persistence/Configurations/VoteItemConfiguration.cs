using EVote360.Domain.Entities;
using EVote360.Domain.Entities.Ballot;
using EVote360.Domain.Entities.Vote;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EVote360.Infrastructure.Persistence.Configurations;

public sealed class VoteItemConfiguration : IEntityTypeConfiguration<VoteItem>
{
    public void Configure(EntityTypeBuilder<VoteItem> builder)
    {
        builder.ToTable("VoteItems");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.VoteId).IsRequired();
        builder.Property(x => x.BallotOptionId).IsRequired();

        builder.HasOne<BallotOption>()
               .WithMany()
               .HasForeignKey(x => x.BallotOptionId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.VoteId, x.BallotOptionId })
               .IsUnique();
    }
}
