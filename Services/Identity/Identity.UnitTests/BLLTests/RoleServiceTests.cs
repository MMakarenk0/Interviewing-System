using AutoMapper;
using DAL_Core.Entities;
using Identity.BLL.Models;
using Identity.BLL.Services;
using Microsoft.AspNetCore.Identity;
using NSubstitute;

namespace Identity.UnitTests.BLLTests;

public class RoleServiceTests
{
    private readonly RoleManager<Role> _roleManager;
    private readonly IMapper _mapper;
    private readonly RoleService _service;

    public RoleServiceTests()
    {
        var store = Substitute.For<IRoleStore<Role>>();
        _roleManager = Substitute.For<RoleManager<Role>>(
            store, null, null, null, null);

        _mapper = Substitute.For<IMapper>();

        _service = new RoleService(_roleManager, _mapper);
    }

    [Fact]
    public async Task GetAllRoleNamesAsync_ShouldReturnNames()
    {
        // Arrange
        var roles = new List<Role>
        {
            new Role { Name = "Admin" },
            new Role { Name = "Candidate" }
        }.AsQueryable();

        _roleManager.Roles.Returns(roles);

        // Act
        var result = await _service.GetAllRoleNamesAsync();

        // Assert
        Assert.Contains("Admin", result);
        Assert.Contains("Candidate", result);
    }

    [Fact]
    public async Task GetRoleByIdAsync_RoleExists_ReturnsDto()
    {
        // Arrange
        var id = Guid.NewGuid();
        var role = new Role { Id = id, Name = "Admin" };
        var dto = new RoleDto { Id = id, Name = "Admin" };

        _roleManager.FindByIdAsync(id.ToString()).Returns(Task.FromResult<Role?>(role));
        _mapper.Map<RoleDto>(role).Returns(dto);

        // Act
        var result = await _service.GetRoleByIdAsync(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Admin", result!.Name);
    }

    [Fact]
    public async Task GetRoleByIdAsync_RoleNotFound_ReturnsNull()
    {
        // Arrange
        var id = Guid.NewGuid();
        _roleManager.FindByIdAsync(id.ToString()).Returns(Task.FromResult<Role?>(null));

        // Act
        var result = await _service.GetRoleByIdAsync(id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateRoleAsync_RoleAlreadyExists_ReturnsFalse()
    {
        // Arrange
        _roleManager.RoleExistsAsync("Admin").Returns(Task.FromResult(true));

        // Act
        var result = await _service.CreateRoleAsync("Admin");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CreateRoleAsync_RoleDoesNotExist_CreatesRoleAndReturnsTrue()
    {
        // Arrange
        _roleManager.RoleExistsAsync("Admin").Returns(Task.FromResult(false));
        _roleManager.CreateAsync(Arg.Any<Role>()).Returns(IdentityResult.Success);

        // Act
        var result = await _service.CreateRoleAsync("Admin");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteRoleAsync_RoleExists_DeletesAndReturnsTrue()
    {
        // Arrange
        var id = Guid.NewGuid();
        var role = new Role { Id = id, Name = "Admin" };

        _roleManager.FindByIdAsync(id.ToString()).Returns(Task.FromResult<Role?>(role));
        _roleManager.DeleteAsync(role).Returns(IdentityResult.Success);

        // Act
        var result = await _service.DeleteRoleAsync(id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteRoleAsync_RoleNotFound_ReturnsFalse()
    {
        // Arrange
        var id = Guid.NewGuid();
        _roleManager.FindByIdAsync(id.ToString()).Returns(Task.FromResult<Role?>(null));

        // Act
        var result = await _service.DeleteRoleAsync(id);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UpdateRoleNameAsync_RoleExists_UpdatesAndReturnsTrue()
    {
        // Arrange
        var id = Guid.NewGuid();
        var model = new UpdateRoleNameDto { RoleId = id, NewName = "Updated" };
        var role = new Role { Id = id, Name = "OldName" };

        _roleManager.FindByIdAsync(id.ToString()).Returns(Task.FromResult<Role?>(role));
        _roleManager.UpdateAsync(role).Returns(IdentityResult.Success);

        // Act
        var result = await _service.UpdateRoleNameAsync(model);

        // Assert
        Assert.True(result);
        Assert.Equal("Updated", role.Name);
    }

    [Fact]
    public async Task UpdateRoleNameAsync_RoleNotFound_ReturnsFalse()
    {
        // Arrange
        var model = new UpdateRoleNameDto { RoleId = Guid.NewGuid(), NewName = "Test" };
        _roleManager.FindByIdAsync(model.RoleId.ToString()).Returns(Task.FromResult<Role?>(null));

        // Act
        var result = await _service.UpdateRoleNameAsync(model);

        // Assert
        Assert.False(result);
    }
}
