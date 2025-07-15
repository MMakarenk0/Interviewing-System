using BFF.API.Middleware;

namespace BFF.API.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseCustomMiddlewares(this IApplicationBuilder app)
    {
        app.UseRateLimiter();
        app.UseHttpsRedirection();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseCors("AllowSpecificOrigins");

        app.UseMiddleware<RefitExceptionMiddleware>();

        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}