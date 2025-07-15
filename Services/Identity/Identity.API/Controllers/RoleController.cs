using Identity.BLL.Models;
using Identity.BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet]
    public async Task<ActionResult<IList<string>>> GetAllRoleNames()
    {
        var roles = await _roleService.GetAllRoleNamesAsync();
        return Ok(roles);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RoleDto>> GetRoleById(Guid id)
    {
        var role = await _roleService.GetRoleByIdAsync(id);
        if (role == null)
            return NotFound(new { message = $"Role with ID={id} not found" });

        return Ok(role);
    }

    [HttpPost]
    public async Task<IActionResult> CreateRole([FromBody] string roleName)
    {
        var result = await _roleService.CreateRoleAsync(roleName);
        return result ? Ok(new { message = "Role created successfully" }) : BadRequest(new { message = "Role already exists" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRole(Guid id)
    {
        var result = await _roleService.DeleteRoleAsync(id);
        return result ? NoContent() : NotFound(new { message = $"Role with ID={id} not found" });
    }

    [HttpPut]
    public async Task<IActionResult> UpdateRoleName([FromBody] UpdateRoleNameDto model)
    {
        var result = await _roleService.UpdateRoleNameAsync(model);
        return result ? Ok(new { message = "Role updated successfully" }) : BadRequest(new { message = "Failed to update role" });
    }
}
