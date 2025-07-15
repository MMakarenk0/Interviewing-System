using AutoMapper;
using DAL_Core.Entities;
using Identity.API.DTOs;
using Identity.API.Services;
using Identity.BLL.Exceptions;
using Identity.BLL.Models;
using Identity.BLL.Services.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using System.Security.Claims;

namespace Identity.UnitTests.BLLTests;

public class AuthServiceTests
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IJwtService _jwtService;
    private readonly IMapper _mapper;
    private readonly AuthService _authService;
    private readonly IPublishEndpoint _publishEndpoint;

    public AuthServiceTests()
    {
        var userStore = Substitute.For<IUserStore<User>>();
        _userManager = Substitute.For<UserManager<User>>(userStore, null, null, null, null, null, null, null, null);

        var roleStore = Substitute.For<IRoleStore<Role>>();
        _roleManager = Substitute.For<RoleManager<Role>>(roleStore, null, null, null, null);

        _jwtService = Substitute.For<IJwtService>();
        _mapper = Substitute.For<IMapper>();

        _publishEndpoint = Substitute.For<IPublishEndpoint>();

        _authService = new AuthService(_userManager, _jwtService, _roleManager, _mapper, _publishEndpoint);
    }

    #region LoginAsync tests

    [Fact]
    public async Task LoginAsync_UserNotFound_ThrowsAuthException()
    {
        // Arrange
        var model = new LoginDto { Login = "user", Password = "pass" };
        _userManager.FindByNameAsync(model.Login).Returns((User)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<AuthException>(() => _authService.LoginAsync(model));
        Assert.Equal(401, ex.StatusCode);
        Assert.Equal("Wrong login or password", ex.Message);
    }

    [Fact]
    public async Task LoginAsync_InvalidPassword_ThrowsAuthException()
    {
        // Arrange
        var user = new User();
        var model = new LoginDto { Login = "user", Password = "wrongpass" };

        _userManager.FindByNameAsync(model.Login).Returns(user);
        _userManager.CheckPasswordAsync(user, model.Password).Returns(false);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<AuthException>(() => _authService.LoginAsync(model));
        Assert.Equal(401, ex.StatusCode);
        Assert.Equal("Wrong login or password", ex.Message);
    }

    [Fact]
    public async Task LoginAsync_NoRoles_ThrowsAuthException()
    {
        // Arrange
        var user = new User();
        var model = new LoginDto { Login = "user", Password = "pass" };

        _userManager.FindByNameAsync(model.Login).Returns(user);
        _userManager.CheckPasswordAsync(user, model.Password).Returns(true);
        _userManager.GetRolesAsync(user).Returns(Array.Empty<string>());

        // Act & Assert
        var ex = await Assert.ThrowsAsync<AuthException>(() => _authService.LoginAsync(model));
        Assert.Equal(403, ex.StatusCode);
        Assert.Equal("User has no roles assigned.", ex.Message);
    }

    [Fact]
    public async Task LoginAsync_RoleNotFoundInRoleManager_ThrowsAuthException()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid() };
        var model = new LoginDto { Login = "user", Password = "pass" };
        var roles = new[] { "Candidate" };

        _userManager.FindByNameAsync(model.Login).Returns(user);
        _userManager.CheckPasswordAsync(user, model.Password).Returns(true);
        _userManager.GetRolesAsync(user).Returns(roles);
        _roleManager.FindByNameAsync(roles[0]).Returns((Role)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<AuthException>(() => _authService.LoginAsync(model));
        Assert.Equal(403, ex.StatusCode);
        Assert.Equal("User role not found.", ex.Message);
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsToken()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid() };
        var model = new LoginDto { Login = "user", Password = "pass" };
        var roles = new[] { "Candidate" };
        var role = new Role { Name = "Candidate" };
        var token = "jwt_token";

        _userManager.FindByNameAsync(model.Login).Returns(user);
        _userManager.CheckPasswordAsync(user, model.Password).Returns(true);
        _userManager.GetRolesAsync(user).Returns(roles);
        _roleManager.FindByNameAsync("Candidate").Returns(role);
        _jwtService.GenerateJWT(Arg.Any<JwtUserDto>()).Returns(token);

        // Act
        var result = await _authService.LoginAsync(model);

        // Assert
        Assert.Equal(token, result);
    }

    #endregion

    #region RegisterAsync tests

    [Fact]
    public async Task RegisterAsync_CreateUserFails_ThrowsAuthException()
    {
        // Arrange
        var model = new RegisterDto { Password = "pass" };
        var user = new User();

        _mapper.Map<User>(model).Returns(user);
        var createResult = IdentityResult.Failed(new IdentityError { Description = "Error" });
        _userManager.CreateAsync(user, model.Password).Returns(createResult);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<AuthException>(() => _authService.RegisterAsync(model));
        Assert.Equal(400, ex.StatusCode);
        Assert.Contains("Error", ex.Message);
    }

    [Fact]
    public async Task RegisterAsync_DefaultRoleNotExists_ThrowsAuthExceptionAndDeletesUser()
    {
        // Arrange
        var model = new RegisterDto { Password = "pass" };
        var user = new User();

        _mapper.Map<User>(model).Returns(user);
        _userManager.CreateAsync(user, model.Password).Returns(IdentityResult.Success);
        _roleManager.RoleExistsAsync("Candidate").Returns(false);
        _userManager.DeleteAsync(user).Returns(IdentityResult.Success);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<AuthException>(() => _authService.RegisterAsync(model));
        Assert.Equal(500, ex.StatusCode);
        Assert.Contains("Default role", ex.Message);
        await _userManager.Received().DeleteAsync(user);
    }

    [Fact]
    public async Task RegisterAsync_AddToRoleFails_ThrowsAuthExceptionAndDeletesUser()
    {
        // Arrange
        var model = new RegisterDto { Password = "pass" };
        var user = new User();

        _mapper.Map<User>(model).Returns(user);
        _userManager.CreateAsync(user, model.Password).Returns(IdentityResult.Success);
        _roleManager.RoleExistsAsync("Candidate").Returns(true);
        _userManager.AddToRoleAsync(user, "Candidate").Returns(
            IdentityResult.Failed(new IdentityError { Description = "Role error" }));
        _userManager.DeleteAsync(user).Returns(IdentityResult.Success);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<AuthException>(() => _authService.RegisterAsync(model));
        Assert.Equal(500, ex.StatusCode);
        Assert.Contains("Role assignment failed", ex.Message);
        await _userManager.Received().DeleteAsync(user);
    }

    [Fact]
    public async Task RegisterAsync_RoleNotFoundAfterAssignment_ThrowsAuthExceptionAndDeletesUser()
    {
        // Arrange
        var model = new RegisterDto { Password = "pass" };
        var user = new User();

        _mapper.Map<User>(model).Returns(user);
        _userManager.CreateAsync(user, model.Password).Returns(IdentityResult.Success);
        _roleManager.RoleExistsAsync("Candidate").Returns(true);
        _userManager.AddToRoleAsync(user, "Candidate").Returns(IdentityResult.Success);
        _roleManager.FindByNameAsync("Candidate").Returns((Role)null);
        _userManager.DeleteAsync(user).Returns(IdentityResult.Success);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<AuthException>(() => _authService.RegisterAsync(model));
        Assert.Equal(500, ex.StatusCode);
        Assert.Contains("Role", ex.Message);
        await _userManager.Received().DeleteAsync(user);
    }

    [Fact]
    public async Task RegisterAsync_ValidInput_ReturnsToken()
    {
        // Arrange
        var model = new RegisterDto { Password = "pass" };
        var user = new User { Id = Guid.NewGuid() };
        var role = new Role { Name = "Candidate" };
        var token = "jwt_token";

        _mapper.Map<User>(model).Returns(user);
        _userManager.CreateAsync(user, model.Password).Returns(IdentityResult.Success);
        _roleManager.RoleExistsAsync("Candidate").Returns(true);
        _userManager.AddToRoleAsync(user, "Candidate").Returns(IdentityResult.Success);
        _roleManager.FindByNameAsync("Candidate").Returns(role);
        _jwtService.GenerateJWT(Arg.Any<JwtUserDto>()).Returns(token);

        // Act
        var result = await _authService.RegisterAsync(model);

        // Assert
        Assert.Equal(token, result);
    }


    #endregion

    #region GetCurrentUserAsync tests

    [Fact]
    public async Task GetCurrentUserAsync_InvalidUserId_ThrowsAuthException()
    {
        // Arrange
        var principal = Substitute.For<ClaimsPrincipal>();
        principal.FindFirst("uid").Returns((Claim)null);
        principal.FindFirst(ClaimTypes.NameIdentifier).Returns((Claim)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<AuthException>(() => _authService.GetCurrentUserAsync(principal));
        Assert.Equal(401, ex.StatusCode);
        Assert.Contains("Invalid or missing", ex.Message);
    }

    [Fact]
    public async Task GetCurrentUserAsync_UserNotFound_ThrowsAuthException()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var principal = Substitute.For<ClaimsPrincipal>();
        principal.FindFirst("uid").Returns(new Claim("uid", userId));
        _userManager.FindByIdAsync(userId).Returns((User)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<AuthException>(() => _authService.GetCurrentUserAsync(principal));
        Assert.Equal(404, ex.StatusCode);
        Assert.Equal("User not found.", ex.Message);
    }

    [Fact]
    public async Task GetCurrentUserAsync_UserHasNoRoles_ThrowsAuthException()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var user = new User();
        var principal = Substitute.For<ClaimsPrincipal>();
        principal.FindFirst("uid").Returns(new Claim("uid", userId));
        _userManager.FindByIdAsync(userId).Returns(user);
        _userManager.GetRolesAsync(user).Returns(Array.Empty<string>());

        // Act & Assert
        var ex = await Assert.ThrowsAsync<AuthException>(() => _authService.GetCurrentUserAsync(principal));
        Assert.Equal(403, ex.StatusCode);
        Assert.Equal("User has no roles.", ex.Message);
    }

    [Fact]
    public async Task GetCurrentUserAsync_ValidUser_ReturnsDto()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var user = new User();
        var roles = new[] { "Candidate" };
        var principal = Substitute.For<ClaimsPrincipal>();
        principal.FindFirst("uid").Returns(new Claim("uid", userId));
        _userManager.FindByIdAsync(userId).Returns(user);
        _userManager.GetRolesAsync(user).Returns(roles);

        var dto = new CurrentUserDto { Role = "Candidate" };
        _mapper.Map<CurrentUserDto>(user).Returns(dto);

        // Act
        var result = await _authService.GetCurrentUserAsync(principal);

        // Assert
        Assert.Equal("Candidate", result.Role);
    }

    #endregion
}
