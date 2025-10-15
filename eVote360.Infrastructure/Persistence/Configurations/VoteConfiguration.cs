using EVote360.Domain.Entities.Vote;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EVote360.Infrastructure.Persistence.Configurations;

public class VoteConfiguration : IEntityTypeConfiguration<Vote>
{
    public void Configure(EntityTypeBuilder<Vote> b)
    {
        b.ToTable("Votes");
        b.HasKey(x => x.Id);

        b.Property(x => x.ElectionId).IsRequired();
        b.Property(x => x.CitizenId).IsRequired();
        b.Property(x => x.CastAt).IsRequired();
        b.Property(x => x.ReceiptHash).HasMaxLength(200).IsRequired();

        b.HasIndex(x => new { x.ElectionId, x.CitizenId }).IsUnique(); // 1 voto por elección y ciudadano

        b.HasMany(x => x.Items)
            .WithOne()
            .HasForeignKey(i => i.VoteId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Property(x => x.CreatedAt).IsRequired();
    }
}
