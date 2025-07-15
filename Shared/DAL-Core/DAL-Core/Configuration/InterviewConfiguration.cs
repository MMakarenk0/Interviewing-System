using DAL_Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL_Core.Configuration;

public class InterviewConfiguration : IEntityTypeConfiguration<Interview>
{
    public void Configure(EntityTypeBuilder<Interview> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.MeetingUrl)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(i => i.StartTime)
            .IsRequired();

        builder.Property(i => i.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(i => i.SecretToken)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasOne(i => i.Slot)
            .WithOne(s => s.Interview)
            .HasForeignKey<Interview>(i => i.SlotId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(i => i.Assessment)
            .WithOne(a => a.Interview)
            .HasForeignKey<Assessment>(a => a.InterviewId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

