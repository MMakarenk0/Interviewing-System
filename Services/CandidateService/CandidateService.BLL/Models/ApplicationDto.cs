using DAL_Core.Enum;

namespace CandidateService.BLL.Models;
public class ApplicationDto
{
    public Guid Id { get; set; }

    public Guid CandidateProfileId { get; set; }

    public Guid PositionId { get; set; }
    public string PositionTitle { get; set; }

    public string ResumeUrl { get; set; }

    public DateTime CreatedAt { get; set; }
    public ApplicationStatus Status { get; set; }

    public Guid? InterviewId { get; set; }
}