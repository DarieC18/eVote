using EVote360.Domain.Entities.Ballot;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EVote360.Infrastructure.Persistence.Configurations;

public class ElectionBallotConfiguration : IEntityTypeConfiguration<ElectionBallot>
{
    public void Configure(EntityTypeBuilder<ElectionBallot> b)
    {
        b.ToTable("ElectionBallots");
        b.HasKey(x => x.Id);

        b.Property(x => x.ElectionId).IsRequired();
        b.Property(x => x.PositionId).IsRequired();

        b.HasOne<Domain.Entities.Election.Election>()
            .WithMany()
            .HasForeignKey(x => x.ElectionId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne<Domain.Entities.Position.Position>()
            .WithMany()
            .HasForeignKey(x => x.PositionId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(x => new { x.ElectionId, x.PositionId }).IsUnique(); // snapshot único por puesto
        b.Property(x => x.CreatedAt).IsRequired();
    }
}
