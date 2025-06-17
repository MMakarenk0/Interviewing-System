using DAL_Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL_Core.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(x => x.FirstName).HasMaxLength(100);
        builder.Property(x => x.LastName).HasMaxLength(100);

        builder.HasOne(x => x.CandidateProfile)
            .WithOne(x => x.User)
            .HasForeignKey<CandidateProfile>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Resumes)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.InterviewSlots)
            .WithOne(x => x.Interviewer)
            .HasForeignKey(x => x.InterviewerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Assessments)
            .WithOne(x => x.Interviewer)
            .HasForeignKey(x => x.InterviewerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

