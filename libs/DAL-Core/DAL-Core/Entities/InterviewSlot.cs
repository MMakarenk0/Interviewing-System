namespace DAL_Core.Entities;

public class InterviewSlot : BaseEntity
{
    public Guid PositionId { get; set; }
    public Position Position { get; set; }

    public Guid InterviewerId { get; set; }
    public User Interviewer { get; set; }

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsBooked { get; set; }

    public Interview Interview { get; set; }
}

