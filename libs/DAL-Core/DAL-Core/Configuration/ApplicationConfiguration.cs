using DAL_Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL_Core.Configuration;

public class ApplicationConfiguration : IEntityTypeConfiguration<Application>
{
    public void Configure(EntityTypeBuilder<Application> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Status)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(a => a.AppliedAt)
            .IsRequired();

        builder.HasOne(a => a.Resume)
            .WithMany()
            .HasForeignKey(a => a.ResumeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Position)
            .WithMany()
            .HasForeignKey(a => a.PositionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.CandidateProfile)
            .WithMany(cp => cp.Applications)
            .HasForeignKey(a => a.CandidateProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Interview)
            .WithOne(i => i.Application)
            .HasForeignKey<Interview>(i => i.ApplicationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

