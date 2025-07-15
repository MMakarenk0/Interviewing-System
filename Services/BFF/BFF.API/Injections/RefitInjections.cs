using BFF.Abstraction.Settings;
using BFF.API.Clients;
using BFF.API.Handlers;
using Refit;

namespace BFF.API.Injections;

public static class RefitInjections
{
    public static void AddRefitClients(this IServiceCollection services, IConfiguration configuration)
    {
        // Get settings from configuration
        var identitySettings = configuration
            .GetSection(IdentitySettings.SectionName)
            .Get<IdentitySettings>() ?? throw new Exception("Missing Identity config");

        var candidateServiceSettings = configuration
            .GetSection(CandidateServiceSettings.SectionName)
            .Get<CandidateServiceSettings>() ?? throw new Exception("Missing CandidateService config");

        var interviewServiceSettings = configuration
            .GetSection(InterviewServiceSettings.SectionName)
            .Get<InterviewServiceSettings>() ?? throw new Exception("Missing InterviewService config");

        // Identity Clients registration
        services.AddRefitClient<IIdentityApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(identitySettings.BaseAddress))
            .AddHttpMessageHandler<JwtTokenHandler>();

        // CandidateService Clients registration
        services.AddRefitClient<ICandidateServiceApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(candidateServiceSettings.BaseAddress))
            .AddHttpMessageHandler<JwtTokenHandler>();

        // InterviewService Clients registration
        services.AddRefitClient<IInterviewServiceApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(interviewServiceSettings.BaseAddress))
            .AddHttpMessageHandler<JwtTokenHandler>();
    }
}