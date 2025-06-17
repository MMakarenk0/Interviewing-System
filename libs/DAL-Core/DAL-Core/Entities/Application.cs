namespace DAL_Core.Entities;

public class Application : BaseEntity
{
    public Guid CandidateProfileId { get; set; }
    public CandidateProfile CandidateProfile { get; set; }

    public Guid PositionId { get; set; }
    public Position Position { get; set; }

    public Guid ResumeId { get; set; }
    public Resume Resume { get; set; }

    public DateTime AppliedAt { get; set; }
    public string Status { get; set; }

    public Interview Interview { get; set; }
}

