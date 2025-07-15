using DAL_Core.Entities;
using Identity.Abstraction.Configuration;
using Identity.BLL.Models;
using Identity.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Identity.BLL.Services;

public class JwtService : IJwtService
{
    private readonly JwtSettings _jwtSettings;
    private readonly RoleManager<Role> _roleManager;
    private const int DefaultTokenExpirationHours = 1; // Default expiration time for JWT

    public JwtService(JwtSettings jwtSettings, RoleManager<Role> roleManager)
    {
        _jwtSettings = jwtSettings;
        _roleManager = roleManager;
    }

    public async Task<string> GenerateJWT(JwtUserDto model)
    {
        var secretKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSettings.Secret.Trim()));

        var signingCredentials = new SigningCredentials(
            secretKey,
            SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, model.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, model.UserId.ToString()),
            new Claim(ClaimTypes.Role, model.RoleName)
        };

        var role = await _roleManager.FindByNameAsync(model.RoleName);
        if (role != null)
        {
            var roleClaims = await _roleManager.GetClaimsAsync(role);
            claims.AddRange(roleClaims.Select(rc => new Claim("permission", rc.Value)));
        }

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(DefaultTokenExpirationHours),
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
