namespace Identity.BLL.Models;

public class UserQueryParameters
{
    public string? Email { get; set; }
    public string? Role { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public bool? LockoutEnabled { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public List<Guid>? CandidateUserIds { get; set; }
}


