namespace BFF.API.Models;

public class AggregatedUserQueryParameters : UserQueryParameters
{
    public string? Position { get; set; }
    public string? YearsOfExperienceRaw { get; set; }
    public string? TechStack { get; set; }
}
