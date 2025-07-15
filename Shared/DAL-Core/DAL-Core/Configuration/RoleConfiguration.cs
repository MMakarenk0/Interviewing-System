using DAL_Core.Entities;
using DAL_Core.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL_Core.Configuration;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasData(
            new Role
            {
                Id = RoleIds.Candidate,
                Name = "Candidate",
                NormalizedName = "CANDIDATE",
                ConcurrencyStamp = RoleIds.Candidate.ToString()
            },
            new Role
            {
                Id = RoleIds.Interviewer,
                Name = "Interviewer",
                NormalizedName = "INTERVIEWER",
                ConcurrencyStamp = RoleIds.Interviewer.ToString()
            },
            new Role
            {
                Id = RoleIds.Administrator,
                Name = "Administrator",
                NormalizedName = "ADMINISTRATOR",
                ConcurrencyStamp = RoleIds.Interviewer.ToString()
            });
    }
}


