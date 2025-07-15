using BFF.API.Handlers;
using BFF.API.Services;
using BFF.API.Services.Interfaces;
using BFF.API.Telemetry;
using Microsoft.ApplicationInsights.Extensibility;

namespace BFF.API.Injections;

public static class BFFInjections
{
    public static void AddBFFServices(
        this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddTransient<JwtTokenHandler>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPositionService, PositionService>();
        services.AddScoped<ICandidateProfileService, CandidateProfileService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IApplicationService, ApplicationService>();


        services.AddSingleton<ITelemetryInitializer>(
            new CloudRoleNameInitializer("BFF")
        );
    }
}
