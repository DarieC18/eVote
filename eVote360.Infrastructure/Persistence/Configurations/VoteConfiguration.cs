using EVote360.Domain.Entities;
using EVote360.Domain.Entities.Vote;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EVote360.Infrastructure.Persistence.Configurations;

public sealed class VoteConfiguration : IEntityTypeConfiguration<Vote>
{
    public void Configure(EntityTypeBuilder<Vote> builder)
    {
        builder.ToTable("Votes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ElectionId).IsRequired();
        builder.Property(x => x.CitizenId).IsRequired();

        builder.HasIndex(x => new { x.ElectionId, x.CitizenId })
               .IsUnique();

        builder.HasMany(x => x.Items)
               .WithOne()
               .HasForeignKey(x => x.VoteId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
