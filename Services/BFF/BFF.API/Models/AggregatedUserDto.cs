namespace BFF.API.Models;

public class AggregatedUserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }

    public CandidateProfileDto? CandidateProfile { get; set; }
}
