namespace InterviewingSystem.Contracts.IntegrationEvents;

public class ApplicationApprovedEvent
{
    public Guid Id { get; set; }
    public Guid CandidateProfileId { get; set; }
    public Guid PositionId { get; set; }
    public DateTime AppliedAt { get; set; }
}

