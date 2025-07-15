using BFF.API.Clients;
using BFF.API.Models;
using BFF.API.Services.Interfaces;

namespace BFF.API.Services;

public class AuthService : IAuthService
{
    private readonly IIdentityApi _identityApi;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(IIdentityApi identityApi, IHttpContextAccessor httpContextAccessor)
    {
        _identityApi = identityApi;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string?> LoginAsync(LoginModel model)
    {
        var token = await _identityApi.LoginAsync(model);

        if (string.IsNullOrWhiteSpace(token))
            return null;

        _httpContextAccessor.HttpContext!.Response.Cookies.Append("jwt", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddHours(1)
        });

        return token;
    }

    public async Task<string?> RegisterAsync(RegisterModel model)
    {
        var token = await _identityApi.RegisterAsync(model);
        if (string.IsNullOrWhiteSpace(token))
            return null;
        _httpContextAccessor.HttpContext!.Response.Cookies.Append("jwt", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddHours(1)
        });
        return token;
    }

    public void Logout()
    {
        _httpContextAccessor.HttpContext?.Response.Cookies.Delete("jwt");
    }

    public async Task<CurrentUserDto?> GetCurrentUserAsync()
    {
        var token = _httpContextAccessor.HttpContext?.Request.Cookies["jwt"];
        if (string.IsNullOrEmpty(token)) return null;

        return await _identityApi.GetGetCurrentUserAsync();
    }
}
