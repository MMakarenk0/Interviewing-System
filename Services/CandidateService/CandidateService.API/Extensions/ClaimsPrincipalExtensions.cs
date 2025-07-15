using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CandidateService.API.Extensions;
public static class ClaimsPrincipalExtensions
{
    public static Guid? GetUserId(this ClaimsPrincipal user)
    {
        var idStr = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                 ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return Guid.TryParse(idStr, out var id) ? id : null;
    }

    public static bool IsAdmin(this ClaimsPrincipal user)
    {
        return user.IsInRole("Administrator");
    }

    public static bool IsInterviewer(this ClaimsPrincipal user)
    {
        return user.IsInRole("Interviewer");
    }
}
