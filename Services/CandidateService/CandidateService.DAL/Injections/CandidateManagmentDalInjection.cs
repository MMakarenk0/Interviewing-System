using CandidateService.DAL.Repositories;
using CandidateService.DAL.Repositories.Interfaces;
using CandidateService.DAL.UoF;
using DAL_Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CandidateService.DAL.Injections;
public static class CandidateManagmentDalInjection
{
    public static IServiceCollection AddCandidateManagmentDalLogic(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<InterviewingSystemDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DbConnectionString")));

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<ICandidateProfileRepository, CandidateProfileRepository>();
        services.AddScoped<IApplicationRepository, ApplicationRepository>();
        services.AddScoped<ICachedPositionRepository, CachedPositionRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}

