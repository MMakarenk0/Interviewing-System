using CandidateService.BLL.Services.Interfaces;
using InterviewingSystem.Contracts.IntegrationEvents;
using MassTransit;

namespace CandidateService.API.Consumers;

public class InterviewScheduledConsumer : IConsumer<InterviewScheduledEvent>
{
    private readonly IApplicationService _applicationService;

    public InterviewScheduledConsumer(IApplicationService applicationService)
    {
        _applicationService = applicationService;
    }

    public async Task Consume(ConsumeContext<InterviewScheduledEvent> context)
    {
        var @event = context.Message;

        await _applicationService.ScheduleInterviewAsync(
            @event.ApplicationId,
            @event.Id
        );
    }
}
