using DAL_Core.Enum;

namespace InterviewService.BLL.Models;
public class CreateInterviewDto
{
    public Guid Id { get; set; }

    public Guid ApplicationId { get; set; }
    public Guid SlotId { get; set; }

    public string MeetingUrl { get; set; }
    public string SecretToken { get; set; }

    public InterviewStatus Status { get; set; } = InterviewStatus.Scheduled;
}
