using Identity.API.DTOs;
using Identity.BLL.Models;
using System.Security.Claims;

namespace Identity.BLL.Services.Interfaces;

public interface IAuthService
{
    Task<CurrentUserDto> GetCurrentUserAsync(ClaimsPrincipal principal);
    Task<string> LoginAsync(LoginDto model);
    Task<string> RegisterAsync(RegisterDto model);
}