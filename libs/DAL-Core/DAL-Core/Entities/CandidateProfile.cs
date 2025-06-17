namespace DAL_Core.Entities;

public class CandidateProfile : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; }

    public ICollection<Application> Applications { get; set; }
}

