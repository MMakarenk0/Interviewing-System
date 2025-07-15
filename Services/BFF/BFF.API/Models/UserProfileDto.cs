namespace BFF.API.Models;

public class UserProfileDto
{
    public CurrentUserDto User { get; set; }
    public CandidateProfileDto? CandidateProfile { get; set; }
}
