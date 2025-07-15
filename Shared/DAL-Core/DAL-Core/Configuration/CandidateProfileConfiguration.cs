using DAL_Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL_Core.Configuration;

public class CandidateProfileConfiguration : IEntityTypeConfiguration<CandidateProfile>
{
    public void Configure(EntityTypeBuilder<CandidateProfile> builder)
    {
        builder.HasKey(cp => cp.Id);

        builder.Property(cp => cp.YearsOfExperience);

        builder.Property(cp => cp.CurrentPosition)
            .HasMaxLength(100);

        builder.Property(cp => cp.TechStack)
            .HasMaxLength(500);

        builder.Property(a => a.ProfileResumeBlobPath)
            .HasMaxLength(100);
    }
}

