using Identity.BLL.Models;

namespace Identity.BLL.Services;

public interface IRoleService
{
    Task<bool> CreateRoleAsync(string roleName);
    Task<bool> DeleteRoleAsync(Guid roleId);
    Task<IList<string>> GetAllRoleNamesAsync();
    Task<RoleDto?> GetRoleByIdAsync(Guid id);
    Task<bool> UpdateRoleNameAsync(UpdateRoleNameDto model);
}