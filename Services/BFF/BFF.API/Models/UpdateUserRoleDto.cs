namespace BFF.API.Models;

public class UpdateUserRoleDto
{
    public Guid UserId { get; set; }
    public string NewRole { get; set; }
}