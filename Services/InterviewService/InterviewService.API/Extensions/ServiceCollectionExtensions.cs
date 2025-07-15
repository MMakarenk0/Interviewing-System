using InterviewService.API.Consumers;
using InterviewService.API.Telemetry;
using MassTransit;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;

namespace InterviewService.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTelemetry(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplicationInsightsTelemetry(options =>
        {
            options.ConnectionString = configuration["ApplicationInsights:ConnectionString"];
        });

        services.AddApplicationInsightsTelemetryProcessor<IgnoreServiceBusTelemetryProcessor>();
        services.AddSingleton<ITelemetryInitializer>(
            new CloudRoleNameInitializer("InterviewService"));

        return services;
    }

    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "InterviewService API", Version = "v1" });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(opt =>
        {
            opt.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["JwtConfig:Issuer"],
                ValidAudience = configuration["JwtConfig:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuration["JwtConfig:Secret"]!)),
                RoleClaimType = ClaimTypes.Role
            };
        });

        return services;
    }

    public static IServiceCollection AddMassTransitWithConsumers(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();

            busConfigurator.AddConsumer<ApplicationApprovedConsumer>();
            busConfigurator.AddConsumer<ApplicationFinalizedConsumer>();

            busConfigurator.UsingAzureServiceBus((context, cfg) =>
            {
                cfg.Host(configuration.GetConnectionString("AzureServiceBus"));

                cfg.SubscriptionEndpoint(
                    subscriptionName: "interview-on-application-approved-sub",
                    topicPath: "candidate-topic",
                    configure: e => e.ConfigureConsumer<ApplicationApprovedConsumer>(context));

                cfg.SubscriptionEndpoint(
                    subscriptionName: "interview-on-application-finalized-sub",
                    topicPath: "candidate-topic",
                    configure: e => e.ConfigureConsumer<ApplicationFinalizedConsumer>(context));
            });
        });

        return services;
    }

    public static IServiceCollection AddCustomControllers(this IServiceCollection services)
    {
        services.AddControllers();
        return services;
    }
}