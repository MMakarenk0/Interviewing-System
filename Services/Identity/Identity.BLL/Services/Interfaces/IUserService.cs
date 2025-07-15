using Identity.BLL.Models;

namespace Identity.BLL.Services.Interfaces;

public interface IUserService
{
    Task<UserDto> CreateUserAsync(CreateUserDto model);
    Task<bool> DeleteUserAsync(Guid id);
    Task<PaginatedResult<UserDto>> GetAllUsersAsync(UserQueryParameters query);
    Task<UserDto> GetUserByIdAsync(Guid id);
    Task<bool> UpdateUserRoleAsync(UpdateUserRoleDto model);
}