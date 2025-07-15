namespace DAL_Core.Entities;

public class CachedApplication : BaseEntity
{
    public Guid Id { get; set; }
    public Guid CandidateProfileId { get; set; }
    public Guid PositionId { get; set; }
    public DateTime UpdatedAt { get; set; }
}

