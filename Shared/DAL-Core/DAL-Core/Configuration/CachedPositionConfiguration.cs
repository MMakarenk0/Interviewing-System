using DAL_Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL_Core.Configuration;

public class CachedPositionConfiguration : IEntityTypeConfiguration<CachedPosition>
{
    public void Configure(EntityTypeBuilder<CachedPosition> builder)
    {
        builder.HasKey(cp => cp.Id);

        builder.Property(cp => cp.Title)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(cp => cp.IsActive)
            .IsRequired();

        builder.Property(cp => cp.UpdatedAt)
            .IsRequired();
    }
}

