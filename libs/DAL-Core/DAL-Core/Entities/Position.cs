namespace DAL_Core.Entities;

public class Position : BaseEntity
{
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<Application> Applications { get; set; }
    public ICollection<InterviewSlot> InterviewSlots { get; set; }
}

