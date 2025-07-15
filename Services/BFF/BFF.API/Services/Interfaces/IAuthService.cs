using BFF.API.Models;

namespace BFF.API.Services.Interfaces;

public interface IAuthService
{
    Task<CurrentUserDto?> GetCurrentUserAsync();
    Task<string?> LoginAsync(LoginModel model);
    void Logout();
    Task<string?> RegisterAsync(RegisterModel model);
}
