namespace DAL_Core.Entities;

public class Interview : BaseEntity
{
    public Guid ApplicationId { get; set; }
    public Application Application { get; set; }

    public Guid SlotId { get; set; }
    public InterviewSlot Slot { get; set; }

    public string MeetingUrl { get; set; }
    public string Status { get; set; }
    public string SecretToken { get; set; }

    public Assessment Assessment { get; set; }
}

