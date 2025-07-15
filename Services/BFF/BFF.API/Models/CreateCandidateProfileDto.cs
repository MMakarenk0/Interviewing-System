namespace BFF.API.Models;

public class CreateCandidateProfileDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public IFormFile? ProfileResumeFile { get; set; }
    public int? YearsOfExperience { get; set; }
    public string? CurrentPosition { get; set; }
    public string? TechStack { get; set; }
    public ICollection<Guid>? ApplicationIds { get; set; }
}