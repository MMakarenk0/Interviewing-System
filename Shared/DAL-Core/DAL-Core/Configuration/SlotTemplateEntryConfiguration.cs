using DAL_Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL_Core.Configuration;

public class SlotTemplateEntryConfiguration : IEntityTypeConfiguration<SlotTemplateEntry>
{
    public void Configure(EntityTypeBuilder<SlotTemplateEntry> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.DayOfWeek)
            .IsRequired();

        builder.Property(e => e.StartTime)
            .IsRequired();

        builder.Property(e => e.EndTime)
            .IsRequired();

        builder.HasOne(e => e.InterviewSlotTemplate)
            .WithMany(t => t.Entries)
            .HasForeignKey(e => e.InterviewSlotTemplateId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

