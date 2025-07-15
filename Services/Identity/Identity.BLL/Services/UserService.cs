using AutoMapper;
using DAL_Core.Entities;
using Identity.BLL.Exceptions;
using Identity.BLL.Models;
using Identity.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Identity.BLL.Services;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IMapper _mapper;

    public UserService(
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        IMapper mapper)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _mapper = mapper;
    }

    public async Task<PaginatedResult<UserDto>> GetAllUsersAsync(UserQueryParameters query)
    {
        var usersQuery = _userManager.Users.AsQueryable();

        // Apply basic filters
        if (!string.IsNullOrWhiteSpace(query.Email))
        {
            usersQuery = usersQuery.Where(u => u.Email.Contains(query.Email));
        }
        if (!string.IsNullOrWhiteSpace(query.FirstName))
        {
            usersQuery = usersQuery.Where(u => u.FirstName.Contains(query.FirstName));
        }
        if (!string.IsNullOrWhiteSpace(query.LastName))
        {
            usersQuery = usersQuery.Where(u => u.LastName.Contains(query.LastName));
        }
        if (query.LockoutEnabled.HasValue)
        {
            usersQuery = usersQuery.Where(u => u.LockoutEnabled == query.LockoutEnabled.Value);
        }

        var allFilteredUsers = await usersQuery.ToListAsync();

        // Filter by candidate IDs if provided
        if (query.CandidateUserIds != null && query.CandidateUserIds.Any())
        {
            if (query.Role == "Candidate")
            {
                allFilteredUsers = allFilteredUsers
                    .Where(u => query.CandidateUserIds.Contains(u.Id))
                    .ToList();
            }
            else
            {
                var result = new List<User>();
                foreach (var user in allFilteredUsers)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    if (query.CandidateUserIds.Contains(user.Id) || !roles.Contains("Candidate"))
                    {
                        result.Add(user);
                    }
                }

                allFilteredUsers = result;
            }
        }

        // Filter by role if specified
        if (!string.IsNullOrWhiteSpace(query.Role))
        {
            allFilteredUsers = await FilterByRoleAsync(allFilteredUsers, query.Role);
        }

        var totalCount = allFilteredUsers.Count;

        var pagedUsers = allFilteredUsers
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToList();

        var resultDto = new PaginatedResult<UserDto>
        {
            TotalCount = totalCount,
            Items = new List<UserDto>()
        };

        // TODO: Consider using Task.WhenAll for better performance
        foreach (var user in pagedUsers)
        {
            var dto = _mapper.Map<UserDto>(user);
            var roles = await _userManager.GetRolesAsync(user);
            dto.Role = roles.FirstOrDefault() ?? "None";
            resultDto.Items.Add(dto);
        }

        return resultDto;
    }


    public async Task<UserDto> GetUserByIdAsync(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
            throw new ArgumentException($"User with ID={id} not found");

        var dto = _mapper.Map<UserDto>(user);
        var roles = await _userManager.GetRolesAsync(user);
        dto.Role = roles.FirstOrDefault() ?? "None";

        return dto;
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto model)
    {
        var existingUser = await _userManager.FindByEmailAsync(model.Email);
        if (existingUser != null)
        {
            throw new ConflictException($"A user with email '{model.Email}' already exists.");
        }

        if (string.IsNullOrWhiteSpace(model.Role))
        {
            throw new ArgumentException("Role is required for admin-created users.");
        }

        var role = await _roleManager.FindByNameAsync(model.Role);
        if (role == null)
        {
            throw new NotFoundException("Role", model.Id);
        }

        var user = _mapper.Map<User>(model);
        user.Id = Guid.NewGuid();
        user.UserName = model.Email;
        user.EmailConfirmed = true;

        var createResult = await _userManager.CreateAsync(user, model.Password);
        if (!createResult.Succeeded)
        {
            var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"User creation failed: {errors}");
        }

        var addToRoleResult = await _userManager.AddToRoleAsync(user, model.Role);
        if (!addToRoleResult.Succeeded)
        {
            var errors = string.Join(", ", addToRoleResult.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to assign role: {errors}");
        }

        return _mapper.Map<UserDto>(user);
    }

    public async Task<bool> DeleteUserAsync(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
            return false;

        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> UpdateUserRoleAsync(UpdateUserRoleDto model)
    {
        var user = await _userManager.FindByIdAsync(model.UserId.ToString());
        if (user == null)
            return false;

        var currentRoles = await _userManager.GetRolesAsync(user);
        var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
        if (!removeResult.Succeeded)
            return false;

        if (!await _roleManager.RoleExistsAsync(model.NewRole))
            return false;

        var addResult = await _userManager.AddToRoleAsync(user, model.NewRole);
        return addResult.Succeeded;
    }

    private async Task<List<User>> FilterByRoleAsync(List<User> users, string role)
    {
        var result = new List<User>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains(role))
            {
                result.Add(user);
            }
        }
        return result;
    }
}
