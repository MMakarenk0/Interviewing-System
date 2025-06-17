using DAL_Core.Configuration;
using DAL_Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL_Core;

public class InterviewingSystemDbContext : IdentityDbContext<User, Role, Guid>
{
    public InterviewingSystemDbContext(DbContextOptions<InterviewingSystemDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }

    public DbSet<Resume> Resumes { get; set; }
    public DbSet<Position> Positions { get; set; }
    public DbSet<InterviewSlot> InterviewSlots { get; set; }
    public DbSet<Interview> Interviews { get; set; }
    public DbSet<Assessment> Assessments { get; set; }
    public DbSet<Application> Applications { get; set; }
    public DbSet<CandidateProfile> CandidateProfiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new ResumeConfiguration());
        modelBuilder.ApplyConfiguration(new PositionConfiguration());
        modelBuilder.ApplyConfiguration(new InterviewSlotConfiguration());
        modelBuilder.ApplyConfiguration(new InterviewConfiguration());
        modelBuilder.ApplyConfiguration(new AssessmentConfiguration());
        modelBuilder.ApplyConfiguration(new ApplicationConfiguration());
        modelBuilder.ApplyConfiguration(new CandidateProfileConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}

