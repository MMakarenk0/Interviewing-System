using Identity.BLL.Models;

namespace Identity.BLL.Services.Interfaces;

public interface IJwtService
{
    Task<string> GenerateJWT(JwtUserDto model);
}