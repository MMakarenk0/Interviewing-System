namespace DAL_Core.Entities;

public class SlotTemplateEntry : BaseEntity
{
    public Guid InterviewSlotTemplateId { get; set; }

    public InterviewSlotTemplate InterviewSlotTemplate { get; set; }

    public DayOfWeek DayOfWeek { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }
}
