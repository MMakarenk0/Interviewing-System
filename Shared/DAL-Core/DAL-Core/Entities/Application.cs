using DAL_Core.Enum;

namespace DAL_Core.Entities;

public class Application : BaseEntity
{
    public Guid CandidateProfileId { get; set; }
    public CandidateProfile? CandidateProfile { get; set; }

    public Guid PositionId { get; set; }

    public ApplicationStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }

    public Guid? InterviewId { get; set; }
    public string? ResumeBlobPath { get; set; }
}

