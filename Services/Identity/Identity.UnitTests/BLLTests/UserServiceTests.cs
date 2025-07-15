using AutoMapper;
using DAL_Core.Entities;
using Identity.BLL.Models;
using Identity.BLL.Services;
using Microsoft.AspNetCore.Identity;
using NSubstitute;

namespace Identity.UnitTests.BLLTests;

public class UserServiceTests
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IMapper _mapper;
    private readonly UserService _service;

    public UserServiceTests()
    {
        var userStore = Substitute.For<IUserStore<User>>();
        _userManager = Substitute.For<UserManager<User>>(userStore, null, null, null, null, null, null, null, null);

        var roleStore = Substitute.For<IRoleStore<Role>>();
        _roleManager = Substitute.For<RoleManager<Role>>(roleStore, null, null, null, null);

        _mapper = Substitute.For<IMapper>();

        _service = new UserService(_userManager, _roleManager, _mapper);
    }

    [Fact]
    public async Task GetAllUsersAsync_ReturnsMappedUsersWithRoles()
    {
        // Arrange
        var users = new List<User>
        {
            new() { Id = Guid.NewGuid() },
            new() { Id = Guid.NewGuid() }
        };
        _userManager.Users.Returns(users.AsQueryable());

        _mapper.Map<UserDto>(Arg.Any<User>()).Returns(callInfo =>
        {
            var user = callInfo.Arg<User>();
            return new UserDto { Id = user.Id };
        });

        foreach (var user in users)
        {
            _userManager.GetRolesAsync(user).Returns(Task.FromResult<IList<string>>(new List<string> { "Role1" }));
        }

        // Act
        var result = await _service.GetAllUsersAsync();

        // Assert
        Assert.Equal(users.Count, result.Count);
        Assert.All(result, dto => Assert.Equal("Role1", dto.Role));
    }

    [Fact]
    public async Task GetUserByIdAsync_UserExists_ReturnsMappedUserWithRole()
    {
        // Arrange
        var id = Guid.NewGuid();
        var user = new User { Id = id };
        _userManager.FindByIdAsync(id.ToString()).Returns(Task.FromResult<User?>(user));

        _mapper.Map<UserDto>(user).Returns(new UserDto { Id = id });
        _userManager.GetRolesAsync(user).Returns(Task.FromResult<IList<string>>(new List<string> { "Admin" }));

        // Act
        var result = await _service.GetUserByIdAsync(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal("Admin", result.Role);
    }


    [Fact]
    public async Task GetUserByIdAsync_UserNotFound_ThrowsArgumentException()
    {
        // Arrange
        var id = Guid.NewGuid();
        _userManager.FindByIdAsync(id.ToString()).Returns(Task.FromResult<User?>(null));

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.GetUserByIdAsync(id));
    }

    [Fact]
    public async Task DeleteUserAsync_UserExists_ReturnsTrue()
    {
        // Arrange
        var id = Guid.NewGuid();
        var user = new User { Id = id };
        _userManager.FindByIdAsync(id.ToString()).Returns(Task.FromResult<User?>(user));
        _userManager.DeleteAsync(user).Returns(Task.FromResult(IdentityResult.Success));

        // Act
        var result = await _service.DeleteUserAsync(id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteUserAsync_UserNotFound_ReturnsFalse()
    {
        // Arrange
        var id = Guid.NewGuid();
        _userManager.FindByIdAsync(id.ToString()).Returns(Task.FromResult<User?>(null));

        // Act
        var result = await _service.DeleteUserAsync(id);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UpdateUserRoleAsync_Success()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var model = new UpdateUserRoleDto { UserId = userId, NewRole = "Admin" };
        var user = new User { Id = userId };

        _userManager.FindByIdAsync(userId.ToString()).Returns(Task.FromResult<User?>(user));
        _userManager.GetRolesAsync(user).Returns(Task.FromResult<IList<string>>(new List<string> { "User" }));
        _userManager.RemoveFromRolesAsync(user, Arg.Any<IEnumerable<string>>()).Returns(Task.FromResult(IdentityResult.Success));
        _roleManager.RoleExistsAsync(model.NewRole).Returns(Task.FromResult(true));
        _userManager.AddToRoleAsync(user, model.NewRole).Returns(Task.FromResult(IdentityResult.Success));

        // Act
        var result = await _service.UpdateUserRoleAsync(model);

        // Assert
        Assert.True(result);
        await _userManager.Received().RemoveFromRolesAsync(user, Arg.Any<IEnumerable<string>>());
        await _userManager.Received().AddToRoleAsync(user, model.NewRole);
    }

    [Fact]
    public async Task UpdateUserRoleAsync_UserNotFound_ReturnsFalse()
    {
        // Arrange
        var model = new UpdateUserRoleDto { UserId = Guid.NewGuid(), NewRole = "Admin" };
        _userManager.FindByIdAsync(model.UserId.ToString()).Returns(Task.FromResult<User?>(null));

        // Act
        var result = await _service.UpdateUserRoleAsync(model);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UpdateUserRoleAsync_RemoveRolesFails_ReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var model = new UpdateUserRoleDto { UserId = userId, NewRole = "Admin" };
        var user = new User { Id = userId };

        _userManager.FindByIdAsync(userId.ToString()).Returns(Task.FromResult<User?>(user));
        _userManager.GetRolesAsync(user).Returns(Task.FromResult<IList<string>>(new List<string> { "User" }));
        _userManager.RemoveFromRolesAsync(user, Arg.Any<IEnumerable<string>>()).Returns(Task.FromResult(IdentityResult.Failed(new IdentityError())));

        // Act
        var result = await _service.UpdateUserRoleAsync(model);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UpdateUserRoleAsync_RoleDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var model = new UpdateUserRoleDto { UserId = userId, NewRole = "Admin" };
        var user = new User { Id = userId };

        _userManager.FindByIdAsync(userId.ToString()).Returns(Task.FromResult<User?>(user));
        _userManager.GetRolesAsync(user).Returns(Task.FromResult<IList<string>>(new List<string> { "User" }));
        _userManager.RemoveFromRolesAsync(user, Arg.Any<IEnumerable<string>>()).Returns(Task.FromResult(IdentityResult.Success));
        _roleManager.RoleExistsAsync(model.NewRole).Returns(Task.FromResult(false));

        // Act
        var result = await _service.UpdateUserRoleAsync(model);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UpdateUserRoleAsync_AddToRoleFails_ReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var model = new UpdateUserRoleDto { UserId = userId, NewRole = "Admin" };
        var user = new User { Id = userId };

        _userManager.FindByIdAsync(userId.ToString()).Returns(Task.FromResult<User?>(user));
        _userManager.GetRolesAsync(user).Returns(Task.FromResult<IList<string>>(new List<string> { "User" }));
        _userManager.RemoveFromRolesAsync(user, Arg.Any<IEnumerable<string>>()).Returns(Task.FromResult(IdentityResult.Success));
        _roleManager.RoleExistsAsync(model.NewRole).Returns(Task.FromResult(true));
        _userManager.AddToRoleAsync(user, model.NewRole).Returns(Task.FromResult(IdentityResult.Failed(new IdentityError())));

        // Act
        var result = await _service.UpdateUserRoleAsync(model);

        // Assert
        Assert.False(result);
    }
}