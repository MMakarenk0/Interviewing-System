using CandidateService.Abstraction.Settings;
using CandidateService.BLL.Clients;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace CandidateService.BLL.Injections;

public static class RefitInjection
{
    public static IServiceCollection AddRefitClients(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var interviewServiceSettings = configuration
            .GetSection(InterviewServiceSettings.SectionName)
            .Get<InterviewServiceSettings>() ?? throw new Exception("Missing InterviewService config");

        // InterviewService Clients registration
        services.AddRefitClient<IPositionClient>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(interviewServiceSettings.BaseAddress));

        return services;
    }
}