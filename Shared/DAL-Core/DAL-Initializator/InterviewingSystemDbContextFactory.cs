using DAL_Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DAL_Initializator;

public class InterviewingSystemDbContextFactory : IDesignTimeDbContextFactory<InterviewingSystemDbContext>
{
    public InterviewingSystemDbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<InterviewingSystemDbContext>();
        optionsBuilder.UseSqlServer(config.GetConnectionString("DbConnectionString"),
            b => b.MigrationsAssembly("DAL-Initializator"));

        return new InterviewingSystemDbContext(optionsBuilder.Options);
    }
}

