using DAL_Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.DAL;

public static class IdentityDalInjection
{
    public static IServiceCollection AddIdentityDalLogic(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<InterviewingSystemDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DbConnectionString")));

        return services;
    }
}
