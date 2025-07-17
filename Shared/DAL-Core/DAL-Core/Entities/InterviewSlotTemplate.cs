namespace DAL_Core.Entities;

public class InterviewSlotTemplate : BaseEntity
{
    public string Name { get; set; }

    public ICollection<SlotTemplateEntry> Entries { get; set; }

    public ICollection<InterviewerProfile> InterviewerProfiles { get; set; }
}
