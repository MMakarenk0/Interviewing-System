using InterviewService.BLL.Profile;
using InterviewService.BLL.Services;
using InterviewService.BLL.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace InterviewService.BLL.Injections;
public static class InterviewManagmentBllInjection
{
    public static IServiceCollection AddInterviewManagmentBusinessLogic(
        this IServiceCollection services)
    {
        services.AddScoped<IInterviewService, Services.InterviewService>();
        services.AddScoped<IPositionService, PositionService>();
        services.AddScoped<IInterviewSlotService, InterviewSlotService>();
        services.AddScoped<IAssessmentService, AssessmentService>();

        services.AddAutoMapper(typeof(InterviewManagmentMappingProfile));

        return services;
    }
}

