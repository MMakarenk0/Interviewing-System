using DAL_Core;
using InterviewService.DAL.Repositories;
using InterviewService.DAL.Repositories.Interfaces;
using InterviewService.DAL.UoF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InterviewService.DAL.Injections;
public static class InterviewManagmentDallInjection
{
    public static IServiceCollection AddInterviewManagmentDalLogic(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<InterviewingSystemDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DbConnectionString")));

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IInterviewSlotRepository, InterviewSlotRepository>();
        services.AddScoped<IInterviewRepository, InterviewRepository>();
        services.AddScoped<IPositionRepository, PositionRepository>();
        services.AddScoped<IAssessmentRepository, AssessmentRepository>();
        services.AddScoped<ICachedApplicationRepository, CachedApplicationRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}

