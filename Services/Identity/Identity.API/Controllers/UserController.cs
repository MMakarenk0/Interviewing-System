using Identity.BLL.Models;
using Identity.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("filter")]
    public async Task<ActionResult<PaginatedResult<UserDto>>> FilterUsers([FromBody] UserQueryParameters query)
    {
        var result = await _userService.GetAllUsersAsync(query);
        return Ok(result);
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUserById(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _userService.CreateUserAsync(model);
        return CreatedAtAction(nameof(GetUserById), new { id = result.Id }, result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var result = await _userService.DeleteUserAsync(id);
        return result ? NoContent() : NotFound();
    }

    [HttpPut]
    public async Task<IActionResult> UpdateUserRole([FromBody] UpdateUserRoleDto model)
    {
        var result = await _userService.UpdateUserRoleAsync(model);
        return result ? Ok(new { message = "Role updated successfully" }) : BadRequest(new { message = "Failed to update role" });
    }
}
