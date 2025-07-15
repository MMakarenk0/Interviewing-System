using Microsoft.AspNetCore.Http;

namespace CandidateService.BLL.Models;
public class CreateCandidateProfileDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public IFormFile? ProfileResumeFile { get; set; }
    public int? YearsOfExperience { get; set; }
    public string? CurrentPosition { get; set; }
    public string? TechStack { get; set; }
    public List<Guid> ApplicationIds { get; set; } = new List<Guid>();
}

