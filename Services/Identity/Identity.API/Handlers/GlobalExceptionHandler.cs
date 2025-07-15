using Identity.BLL.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Net;

namespace Identity.API.Handlers;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ProblemDetailsFactory _problemDetailsFactory;

    public GlobalExceptionHandler(
        ProblemDetailsFactory problemDetailsFactory)
    {
        _problemDetailsFactory = problemDetailsFactory;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title, detail) = exception switch
        {
            AuthException authEx => (
                authEx.StatusCode,
                "Authentication Error",
                authEx.Message
            ),
            UnauthorizedAccessException => (
                (int)HttpStatusCode.Unauthorized,
                "Unauthorized",
                "Access denied"
            ),
            _ => (
                (int)HttpStatusCode.InternalServerError,
                "Server Error",
                "An unexpected error occurred."
            )
        };

        var problem = _problemDetailsFactory.CreateProblemDetails(
            httpContext,
            statusCode: statusCode,
            title: title,
            detail: detail
        );

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

        return true;
    }
}