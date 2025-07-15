using DAL_Core.Entities;
using Identity.Abstraction.Configuration;
using Identity.BLL.Models;
using Identity.BLL.Services;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Identity.UnitTests.BLLTests;
public class JwtServiceTests
{
    private readonly JwtSettings _jwtSettings;
    private readonly RoleManager<Role> _roleManager;
    private readonly JwtService _jwtService;

    public JwtServiceTests()
    {
        _jwtSettings = new JwtSettings
        {
            Secret = "very_secret_key_which_is_long_enough_1234567890",
            Issuer = "TestIssuer",
            Audience = "TestAudience"
        };

        var store = Substitute.For<IRoleStore<Role>>();
        _roleManager = Substitute.For<RoleManager<Role>>(store, null, null, null, null);

        _jwtService = new JwtService(_jwtSettings, _roleManager);
    }

    [Fact]
    public async Task GenerateJWT_ShouldReturnValidTokenWithClaims()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var roleName = "Admin";

        var model = new JwtUserDto
        {
            UserId = userId,
            RoleName = roleName
        };

        var role = new Role { Name = roleName };

        _roleManager.FindByNameAsync(roleName).Returns(role);

        _roleManager.GetClaimsAsync(role).Returns(new List<Claim>
        {
            new Claim("permission", "read"),
            new Claim("permission", "write")
        });

        // Act
        var tokenString = await _jwtService.GenerateJWT(model);

        // Assert
        Assert.False(string.IsNullOrEmpty(tokenString));

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(tokenString);

        Assert.Contains(token.Claims, c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == userId.ToString());
        Assert.Contains(token.Claims, c => c.Type == ClaimTypes.NameIdentifier && c.Value == userId.ToString());
        Assert.Contains(token.Claims, c => c.Type == ClaimTypes.Role && c.Value == roleName);

        var permissions = token.Claims.Where(c => c.Type == "permission").Select(c => c.Value).ToList();
        Assert.Contains("read", permissions);
        Assert.Contains("write", permissions);

        Assert.True(token.ValidTo > DateTime.UtcNow);
    }

    [Fact]
    public async Task GenerateJWT_ShouldReturnTokenWithoutRoleClaims_IfRoleNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var roleName = "NonExistingRole";

        var model = new JwtUserDto
        {
            UserId = userId,
            RoleName = roleName
        };

        _roleManager.FindByNameAsync(roleName).Returns((Role)null);

        // Act
        var tokenString = await _jwtService.GenerateJWT(model);

        // Assert
        Assert.False(string.IsNullOrEmpty(tokenString));

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(tokenString);

        Assert.Contains(token.Claims, c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == userId.ToString());
        Assert.Contains(token.Claims, c => c.Type == ClaimTypes.Role && c.Value == roleName);

        Assert.DoesNotContain(token.Claims, c => c.Type == "permission");
    }
}
