using DAL_Core.Enum;

namespace DAL_Core.Entities;

public class Interview : BaseEntity
{
    public Guid ApplicationId { get; set; }
    public Guid CandidateProfileId { get; set; }
    public Guid SlotId { get; set; }
    public InterviewSlot Slot { get; set; }

    public DateTime StartTime { get; set; }
    public string MeetingUrl { get; set; }
    public InterviewStatus Status { get; set; }
    public string SecretToken { get; set; }

    public Assessment Assessment { get; set; }
}

