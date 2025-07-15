using Identity.API.Services;
using Identity.BLL.Services;
using Identity.BLL.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.BLL.Injections;

public static class IdentityBlInjection
{
    public static IServiceCollection AddIdentityBusinessLogic(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();

        services.AddAutoMapper(typeof(IdentityBlInjection).Assembly);

        return services;
    }
}

