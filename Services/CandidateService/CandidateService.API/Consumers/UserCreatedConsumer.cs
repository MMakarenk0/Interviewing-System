using CandidateService.BLL.Models;
using CandidateService.BLL.Services.Interfaces;
using InterviewingSystem.Contracts.IntegrationEvents;
using MassTransit;

namespace CandidateService.API.Consumers;

public class UserCreatedConsumer : IConsumer<UserCreatedEvent>
{
    private readonly ICandidateProfileService _candidateProfileService;
    private readonly ILogger<UserCreatedConsumer> _logger;

    public UserCreatedConsumer(ICandidateProfileService candidateProfileService, ILogger<UserCreatedConsumer> logger)
    {
        _candidateProfileService = candidateProfileService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UserCreatedEvent> context)
    {
        var @event = context.Message;

        if (@event.Role != "Candidate")
            return;

        var profileDto = new CreateCandidateProfileDto
        {
            Id = Guid.NewGuid(),
            UserId = @event.Id,
        };

        await _candidateProfileService.CreateCandidateProfileAsync(profileDto);
    }
}
