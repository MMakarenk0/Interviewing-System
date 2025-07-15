using BFF.API.Models;
using Refit;

namespace BFF.API.Clients;

public interface IIdentityApi
{
    #region Auth
    [Post("/api/auth/login")]
    Task<string> LoginAsync(LoginModel model);

    [Post("/api/auth/register")]
    Task<string> RegisterAsync(RegisterModel model);

    [Get("/api/auth/me")]
    Task<CurrentUserDto> GetGetCurrentUserAsync();
    #endregion

    #region User

    [Post("/api/user/filter")]
    Task<PaginatedResult<UserDto>> GetUsersAsync([Body] UserQueryParameters query);


    [Get("/api/user/{id}")]
    Task<UserDto> GetUserByIdAsync(Guid id);

    [Post("/api/user")]
    Task<UserDto> CreateUserAsync([Body] CreateUserDto model);

    [Delete("/api/user/{id}")]
    Task DeleteUserAsync(Guid id);

    [Put("/api/user")]
    Task UpdateUserRoleAsync([Body] UpdateUserRoleDto model);

    #endregion

    #region Role

    #endregion
}