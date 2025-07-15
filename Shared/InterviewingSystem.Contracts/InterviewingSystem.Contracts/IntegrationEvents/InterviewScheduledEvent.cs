namespace InterviewingSystem.Contracts.IntegrationEvents;

public class InterviewScheduledEvent
{
    // For notifications and setting up the interview to application
    public Guid Id { get; set; }
    public DateTime StartTime { get; set; }
    public Guid ApplicationId { get; set; }
    public string MeetingUrl { get; set; }
    public string SecretToken { get; set; }
}

