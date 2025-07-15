using DAL_Core.Enum;

namespace InterviewService.BLL.Models;

public class ApplicationDto
{
    public Guid Id { get; set; }

    public Guid CandidateProfileId { get; set; }
    public string CandidateFullName { get; set; }

    public Guid PositionId { get; set; }
    public string PositionTitle { get; set; }

    public string ResumeUrl { get; set; }

    public ApplicationStatus Status { get; set; }

    public Guid? InterviewId { get; set; }
}