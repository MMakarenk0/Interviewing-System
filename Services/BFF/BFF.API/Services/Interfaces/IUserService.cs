using BFF.API.Models;

namespace BFF.API.Services.Interfaces;

public interface IUserService
{
    Task<UserDto> CreateUserAsync(CreateUserDto model);
    Task DeleteUserAsync(Guid id);
    Task<UserDto> GetUserByIdAsync(Guid id);
    Task<PaginatedResult<AggregatedUserDto>> GetUsersWithCandidateProfiles(AggregatedUserQueryParameters query);
    Task UpdateUserRoleAsync(UpdateUserRoleDto model);
}