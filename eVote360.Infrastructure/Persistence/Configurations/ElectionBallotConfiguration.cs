using EVote360.Domain.Entities.Ballot;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EVote360.Infrastructure.Persistence.Configurations;

public class ElectionBallotConfiguration : IEntityTypeConfiguration<ElectionBallot>
{
    public void Configure(EntityTypeBuilder<ElectionBallot> builder)
    {
        builder.ToTable("ElectionBallots");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ElectionId).IsRequired();
        builder.Property(x => x.PositionId).IsRequired();

        builder.HasIndex(x => new { x.ElectionId, x.PositionId }).IsUnique();
    }
}
