namespace InterviewingSystem.Contracts.IntegrationEvents;

public class PositionCreatedEvent
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
}

