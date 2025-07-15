namespace BFF.API.Models;

public class CandidateProfileDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? ProfileResumeUrl { get; set; }
    public int? YearsOfExperience { get; set; }
    public string? CurrentPosition { get; set; }
    public string? TechStack { get; set; }
    public ICollection<ApplicationDto> Applications { get; set; }
}