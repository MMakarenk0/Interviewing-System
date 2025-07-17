namespace DAL_Core.Entities;

public class InterviewerProfile : BaseEntity
{
    public Guid UserId { get; set; }

    public Guid? SlotTemplateId { get; set; }

    public InterviewSlotTemplate? SlotTemplate { get; set; }
}

