using EVote360.Domain.Entities.Citizen;
using EVote360.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EVote360.Infrastructure.Persistence.Configurations;

public class CitizenConfiguration : IEntityTypeConfiguration<Citizen>
{
    public void Configure(EntityTypeBuilder<Citizen> b)
    {
        b.ToTable("Citizens");
        b.HasKey(x => x.Id);

        b.Property(x => x.FirstName).HasMaxLength(120).IsRequired();
        b.Property(x => x.LastName).HasMaxLength(120).IsRequired();

        // Value Objects
        b.Property(x => x.NationalId)
            .HasConversion(new NationalIdConverter())
            .HasMaxLength(11)
            .IsRequired();

        b.HasIndex(x => x.NationalId).IsUnique();

        b.Property(x => x.Email)
            .HasConversion(new EmailAddressConverter())
            .HasMaxLength(254)
            .IsRequired(false);

        b.Property(x => x.IsActive).IsRequired();
        b.Property(x => x.CreatedAt).IsRequired();
    }
}
