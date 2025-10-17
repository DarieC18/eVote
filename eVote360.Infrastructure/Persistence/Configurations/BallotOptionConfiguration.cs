// eVote360.Infrastructure/Persistence/Configurations/BallotOptionConfiguration.cs
using EVote360.Domain.Entities.Ballot;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EVote360.Infrastructure.Persistence.Configurations;

public class BallotOptionConfiguration : IEntityTypeConfiguration<BallotOption>
{
    public void Configure(EntityTypeBuilder<BallotOption> builder)
    {
        builder.ToTable("BallotOptions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ElectionId).IsRequired();
        builder.Property(x => x.PositionId).IsRequired();
        builder.Property(x => x.IsNinguno).IsRequired();

        builder.Property(x => x.CandidateId);
        builder.Property(x => x.PartyId);

        builder.HasIndex(x => new { x.ElectionId, x.PositionId, x.IsNinguno })
               .IsUnique()
               .HasFilter("[IsNinguno] = 1");

        builder.HasIndex(x => new { x.ElectionId, x.PositionId, x.CandidateId })
               .IsUnique()
               .HasFilter("[CandidateId] IS NOT NULL");
    }
}
