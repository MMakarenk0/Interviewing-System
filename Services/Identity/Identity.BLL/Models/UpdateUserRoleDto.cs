namespace Identity.BLL.Models;

public class UpdateUserRoleDto
{
    public Guid UserId { get; set; }
    public string NewRole { get; set; }
}

