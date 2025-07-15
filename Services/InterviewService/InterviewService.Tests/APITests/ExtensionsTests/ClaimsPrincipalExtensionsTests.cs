using InterviewService.API.Extensions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace InterviewService.Tests.APITests.ExtensionsTests;

public class ClaimsPrincipalExtensionsTests
{
    [Fact]
    public void GetUserId_ShouldReturnGuid_WhenClaimExistsAndValid()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, guid.ToString())
        };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        // Act
        var result = principal.GetUserId();

        // Assert
        Assert.Equal(guid, result);
    }

    [Fact]
    public void GetUserId_ShouldReturnNull_WhenNoClaim()
    {
        // Arrange
        var principal = new ClaimsPrincipal(new ClaimsIdentity());

        // Act
        var result = principal.GetUserId();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetUserId_ShouldReturnNull_WhenClaimInvalid()
    {
        // Arrange
        var claims = new List<Claim> { new Claim(JwtRegisteredClaimNames.Sub, "not-a-guid") };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        // Act
        var result = principal.GetUserId();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void IsAdmin_ShouldReturnTrue_WhenUserHasAdminRole()
    {
        // Arrange
        var claims = new List<Claim> { new Claim(ClaimTypes.Role, "Administrator") };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        // Act & Assert
        Assert.True(principal.IsAdmin());
    }

    [Fact]
    public void IsAdmin_ShouldReturnFalse_WhenUserHasNoAdminRole()
    {
        // Arrange
        var principal = new ClaimsPrincipal(new ClaimsIdentity());

        // Act & Assert
        Assert.False(principal.IsAdmin());
    }
}

