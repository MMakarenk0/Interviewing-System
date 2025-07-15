using CandidateService.BLL.Profile;
using CandidateService.BLL.Services;
using CandidateService.BLL.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CandidateService.BLL.Injections;
public static class CandidateManagmentBlInjection
{
    public static IServiceCollection AddCandidateManagmentBusinessLogic(
        this IServiceCollection services)
    {
        services.AddScoped<ICandidateProfileService, CandidateProfileService>();
        services.AddScoped<IApplicationService, ApplicationService>();
        services.AddScoped<IBlobStorageService, BlobStorageService>();

        services.AddAutoMapper(typeof(CandidateManagmentMappingProfile));

        return services;
    }
}

