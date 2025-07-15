using AutoMapper;
using DAL_Core.Entities;
using Identity.API.DTOs;
using Identity.BLL.Exceptions;
using Identity.BLL.Models;
using Identity.BLL.Services.Interfaces;
using InterviewingSystem.Contracts.IntegrationEvents;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Identity.API.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IJwtService _jwtService;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IMapper _mapper;
    private const string DefaultRole = "Candidate";

    public AuthService(
        UserManager<User> userManager,
        IJwtService jwtService,
        RoleManager<Role> roleManager,
        IMapper mapper,
        IPublishEndpoint publishEndpoint)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _roleManager = roleManager;
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<string> LoginAsync(LoginDto model)
    {
        var user = await _userManager.FindByNameAsync(model.Login);
        if (user == null)
        {
            throw new AuthException("Wrong login or password", 401);
        }
        var isPasswordValid = await _userManager.CheckPasswordAsync(user, model.Password);
        if (!isPasswordValid)
        {
            throw new AuthException("Wrong login or password", 401);
        }

        var roles = await _userManager.GetRolesAsync(user);
        if (roles == null || !roles.Any())
        {
            throw new AuthException("User has no roles assigned.", 403);
        }

        var userRole = roles.First();
        var role = await _roleManager.FindByNameAsync(userRole);

        if (role == null)
        {
            throw new AuthException("User role not found.", 403);
        }

        return await _jwtService.GenerateJWT(new JwtUserDto
        {
            UserId = user.Id,
            RoleName = role.Name
        });
    }

    public async Task<string> RegisterAsync(RegisterDto model)
    {
        var user = _mapper.Map<User>(model);
        user.Id = Guid.NewGuid();

        var createResult = await _userManager.CreateAsync(user, model.Password);
        if (!createResult.Succeeded)
        {
            var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
            throw new AuthException($"Registration failed: {errors}", 400);
        }

        if (!await _roleManager.RoleExistsAsync(DefaultRole))
        {
            await _userManager.DeleteAsync(user);
            throw new AuthException($"Default role '{DefaultRole}' not found.", 500);
        }

        var addToRoleResult = await _userManager.AddToRoleAsync(user, DefaultRole);
        if (!addToRoleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);
            var errors = string.Join(", ", addToRoleResult.Errors.Select(e => e.Description));
            throw new AuthException($"Role assignment failed: {errors}", 500);
        }

        var role = await _roleManager.FindByNameAsync(DefaultRole);
        if (role == null)
        {
            await _userManager.DeleteAsync(user);
            throw new AuthException($"Role '{DefaultRole}' not found after assignment.", 500);
        }

        var token = await _jwtService.GenerateJWT(new JwtUserDto
        {
            UserId = user.Id,
            RoleName = role.Name
        });

        var userCreatedEvent = new UserCreatedEvent
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = role.Name
        };

        await _publishEndpoint.Publish(userCreatedEvent);

        return token;
    }

    public async Task<CurrentUserDto> GetCurrentUserAsync(ClaimsPrincipal principal)
    {
        var userIdStr = principal.FindFirst("uid")?.Value
                     ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
        {
            throw new AuthException("Invalid or missing user ID in token.", 401);
        }

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            throw new AuthException("User not found.", 404);
        }

        var roles = await _userManager.GetRolesAsync(user);
        if (roles == null || !roles.Any())
        {
            throw new AuthException("User has no roles.", 403);
        }

        var dto = _mapper.Map<CurrentUserDto>(user);
        dto.Role = roles.First();

        return dto;
    }
}
