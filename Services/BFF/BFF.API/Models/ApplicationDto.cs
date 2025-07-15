namespace BFF.API.Models;

public class ApplicationDto
{
    public Guid Id { get; set; }

    public Guid CandidateProfileId { get; set; }
    public string CandidateFullName { get; set; }

    public Guid PositionId { get; set; }
    public string PositionTitle { get; set; }

    public string ResumeUrl { get; set; }

    public DateTime CreatedAt { get; set; }
    public string Status { get; set; }

    public Guid? InterviewId { get; set; }
}