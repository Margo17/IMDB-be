using System.Security.Claims;

namespace IMDB.Api.Minimal.Auth;

public static class IdentityExtensions
{
    public static Guid? GetUserId(this HttpContext httpContext)
    {
        Claim? userId = httpContext.User.Claims.SingleOrDefault(c => c.Type == "userid");

        if (Guid.TryParse(userId?.Value, out Guid parsedId))
        {
            return parsedId;
        }

        return null;
    }
}