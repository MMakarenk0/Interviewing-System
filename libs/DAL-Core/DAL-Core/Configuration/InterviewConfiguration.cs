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
            .HasMaxLength(500);

        builder.Property(i => i.Status)
            .HasMaxLength(50);

        builder.Property(i => i.SecretToken)
            .HasMaxLength(100);

        builder.HasOne(i => i.Application)
            .WithMany()
            .HasForeignKey(i => i.ApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(i => i.Slot)
            .WithMany()
            .HasForeignKey(i => i.SlotId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

