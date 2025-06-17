using DAL_Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DAL_Initializator;

// Migration initializer for the Interviewing System project.

// Add-Migration MigrationName -StartupProject DAL-Initializator -Project DAL-Initializator

class Program
{
    static void Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        using (var scope = host.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<InterviewingSystemDbContext>();
            context.Database.Migrate();
        }

        Console.WriteLine("Migrations applied!");
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddUserSecrets<Program>();
            })
            .ConfigureServices((context, services) =>
            {
                services.AddDbContext<InterviewingSystemDbContext>(options =>
                    options.UseSqlServer(context.Configuration.GetConnectionString("DbConnectionString")));
            });
}