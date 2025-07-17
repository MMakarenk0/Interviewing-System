using DAL_Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL_Core.Configuration;

public class InterviewerProfileConfiguration : IEntityTypeConfiguration<InterviewerProfile>
{
    public void Configure(EntityTypeBuilder<InterviewerProfile> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.UserId)
            .IsRequired();

        builder.HasIndex(i => i.UserId)
            .IsUnique();

        builder.HasOne(i => i.SlotTemplate)
            .WithMany(t => t.InterviewerProfiles)
            .HasForeignKey(i => i.SlotTemplateId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}


