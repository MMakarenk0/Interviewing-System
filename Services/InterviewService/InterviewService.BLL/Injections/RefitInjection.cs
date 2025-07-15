using InterviewService.Abstraction.Settings;
using InterviewService.BLL.Clients;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace InterviewService.BLL.Injections;

public static class RefitInjection
{
    public static IServiceCollection AddRefitClients(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var candidateServiceSettings = configuration
            .GetSection(CandidateServiceSettings.SectionName)
            .Get<CandidateServiceSettings>() ?? throw new Exception("Missing CandidateService config");

        // CandidateService Clients registration
        services.AddRefitClient<ICandidateServiceApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(candidateServiceSettings.BaseAddress));
        //.AddHttpMessageHandler<JwtTokenHandler>();

        return services;
    }
}

