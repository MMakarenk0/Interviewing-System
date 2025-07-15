using BFF.API.Models;
using BFF.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BFF.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ICandidateProfileService _candidateProfileService;

    public AuthController(IAuthService authService, ICandidateProfileService candidateProfileService)
    {
        _authService = authService;
        _candidateProfileService = candidateProfileService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var token = await _authService.LoginAsync(model);

        if (string.IsNullOrEmpty(token))
            return Unauthorized("Invalid credentials");

        var options = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddHours(1),
            Path = "/",
        };

        Response.Cookies.Append("jwt", token, options);

        return Ok(token);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        var token = await _authService.RegisterAsync(model);
        if (string.IsNullOrEmpty(token))
            return BadRequest("Registration failed");
        var options = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddHours(1),
            Path = "/",
        };
        Response.Cookies.Append("jwt", token, options);
        return Ok(token);
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var user = await _authService.GetCurrentUserAsync();
        if (user == null)
            return Unauthorized("User not authenticated");

        CandidateProfileDto? candidateProfile = null;

        if (user.Role == "Candidate")
        {
            candidateProfile = await _candidateProfileService.GetCandidateProfileByUserIdAsync(user.Id);
        }

        var userProfile = new UserProfileDto
        {
            User = user,
            CandidateProfile = candidateProfile
        };

        return Ok(userProfile);
    }


    [HttpGet("logout")]
    public IActionResult Logout()
    {
        _authService.Logout();
        return Ok("Logged out successfully");
    }
}
