namespace DAL_Core.Entities;

public class CandidateProfile : BaseEntity
{
    public Guid UserId { get; set; }

    public string? ProfileResumeBlobPath { get; set; }

    public ICollection<Application> Applications { get; set; }

    public int? YearsOfExperience { get; set; }
    public string? CurrentPosition { get; set; }
    public string? TechStack { get; set; }
}

