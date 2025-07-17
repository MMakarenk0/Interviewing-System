using DAL_Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL_Core.Configuration;

public class InterviewSlotTemplateConfiguration : IEntityTypeConfiguration<InterviewSlotTemplate>
{
    public void Configure(EntityTypeBuilder<InterviewSlotTemplate> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .HasMaxLength(100);

        builder.HasMany(t => t.Entries)
            .WithOne(e => e.InterviewSlotTemplate)
            .HasForeignKey(e => e.InterviewSlotTemplateId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

