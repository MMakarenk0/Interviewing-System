﻿using DAL_Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL_Core.Configuration;

public class AssessmentConfiguration : IEntityTypeConfiguration<Assessment>
{
    public void Configure(EntityTypeBuilder<Assessment> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Feedback).HasMaxLength(2000);

        builder.HasOne(a => a.Interview)
            .WithOne(i => i.Assessment)
            .HasForeignKey<Assessment>(a => a.InterviewId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

