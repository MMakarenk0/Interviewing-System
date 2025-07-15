using InterviewingSystem.Contracts.IntegrationEvents;
using MassTransit;

namespace Identity.API.Extensions;

public static class MassTransitExtensions
{
    public static IServiceCollection AddCustomMassTransit(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();

            busConfigurator.UsingAzureServiceBus((context, cfg) =>
            {
                cfg.Host(configuration["ServiceBus:ConnectionString"]);

                cfg.Message<UserCreatedEvent>(x =>
                {
                    x.SetEntityName("identity-topic");
                });
            });
        });

        return services;
    }
}