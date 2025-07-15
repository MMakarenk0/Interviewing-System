namespace BFF.API.Models;

public class CandidateProfileQuery
{
    public List<Guid> UserIds { get; set; } = new();
    public string? Position { get; set; }
    public string? TechStack { get; set; }

    public string? YearsOfExperienceOperator { get; set; } // ">", ">=", etc.
    public int? YearsOfExperienceValue { get; set; }
}