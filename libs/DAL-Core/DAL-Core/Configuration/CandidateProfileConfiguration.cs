using DAL_Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL_Core.Configuration;

public class CandidateProfileConfiguration : IEntityTypeConfiguration<CandidateProfile>
{
    public void Configure(EntityTypeBuilder<CandidateProfile> builder)
    {
        builder.HasKey(cp => cp.Id);

        builder.HasOne(p => p.User)
            .WithOne()
            .HasForeignKey<CandidateProfile>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

