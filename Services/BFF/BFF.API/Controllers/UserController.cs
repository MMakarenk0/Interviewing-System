using BFF.API.Models;
using BFF.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BFF.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResult<AggregatedUserDto>>> GetAllUsers([FromQuery] AggregatedUserQueryParameters query)
    {
        var users = await _userService.GetUsersWithCandidateProfiles(query);
        return Ok(users);
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
        var userId = await _userService.CreateUserAsync(model);
        return CreatedAtAction(nameof(GetUserById), new { id = userId }, null);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        await _userService.DeleteUserAsync(id);
        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> UpdateUserRole([FromBody] UpdateUserRoleDto model)
    {
        await _userService.UpdateUserRoleAsync(model);
        return Ok();
    }
}
