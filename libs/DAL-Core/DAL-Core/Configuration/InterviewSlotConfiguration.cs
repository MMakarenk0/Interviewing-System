using DAL_Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL_Core.Configuration;

public class InterviewSlotConfiguration : IEntityTypeConfiguration<InterviewSlot>
{
    public void Configure(EntityTypeBuilder<InterviewSlot> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(s => s.StartTime)
            .IsRequired();

        builder.Property(s => s.EndTime)
            .IsRequired();

        builder.HasOne(s => s.Position)
            .WithMany()
            .HasForeignKey(s => s.PositionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Interviewer)
            .WithMany()
            .HasForeignKey(s => s.InterviewerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

