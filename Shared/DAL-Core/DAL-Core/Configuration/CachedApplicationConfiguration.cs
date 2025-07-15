using DAL_Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL_Core.Configuration;

public class CachedApplicationConfiguration : IEntityTypeConfiguration<CachedApplication>
{
    public void Configure(EntityTypeBuilder<CachedApplication> builder)
    {
        builder.HasKey(ca => ca.Id);

        builder.Property(ca => ca.CandidateProfileId)
            .IsRequired();

        builder.Property(ca => ca.PositionId)
            .IsRequired();

        builder.Property(ca => ca.UpdatedAt)
            .IsRequired();
    }
}

