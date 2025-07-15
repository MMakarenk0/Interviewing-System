using DAL_Core.Enum;

namespace InterviewService.BLL.Models;
public class InterviewDto
{
    public Guid Id { get; set; }

    public Guid ApplicationId { get; set; }
    public string CandidateFullName { get; set; } // from Application.CandidateProfile.User
    public string PositionTitle { get; set; }     // from Application.Position

    public Guid SlotId { get; set; }
    public DateTime StartTime { get; set; }       // from InterviewSlot
    public DateTime EndTime { get; set; }

    public string MeetingUrl { get; set; }
    public InterviewStatus Status { get; set; }

    public string SecretToken { get; set; }

    public AssessmentDto? Assessment { get; set; }
}

