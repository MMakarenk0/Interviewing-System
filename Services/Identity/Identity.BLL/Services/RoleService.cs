using AutoMapper;
using DAL_Core.Entities;
using Identity.BLL.Models;
using Microsoft.AspNetCore.Identity;

namespace Identity.BLL.Services;

public class RoleService : IRoleService
{
    private readonly RoleManager<Role> _roleManager;
    private readonly IMapper _mapper;

    public RoleService(RoleManager<Role> roleManager, IMapper mapper)
    {
        _roleManager = roleManager;
        _mapper = mapper;
    }

    public async Task<IList<string>> GetAllRoleNamesAsync()
    {
        return _roleManager.Roles.Select(r => r.Name).ToList();
    }

    public async Task<RoleDto?> GetRoleByIdAsync(Guid id)
    {
        var role = await _roleManager.FindByIdAsync(id.ToString());
        return role == null ? null : _mapper.Map<RoleDto>(role);
    }

    public async Task<bool> CreateRoleAsync(string roleName)
    {
        if (await _roleManager.RoleExistsAsync(roleName))
            return false;

        var result = await _roleManager.CreateAsync(new Role { Name = roleName });
        return result.Succeeded;
    }

    public async Task<bool> DeleteRoleAsync(Guid roleId)
    {
        var role = await _roleManager.FindByIdAsync(roleId.ToString());
        if (role == null)
            return false;

        var result = await _roleManager.DeleteAsync(role);
        return result.Succeeded;
    }

    public async Task<bool> UpdateRoleNameAsync(UpdateRoleNameDto model)
    {
        var role = await _roleManager.FindByIdAsync(model.RoleId.ToString());
        if (role == null)
            return false;

        role.Name = model.NewName;
        var result = await _roleManager.UpdateAsync(role);
        return result.Succeeded;
    }
}
