using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Primitives;

namespace IMDB.Api.Auth;

public class AdminAuthRequirement(string _apiKey) : IAuthorizationHandler, IAuthorizationRequirement
{
    public Task HandleAsync(AuthorizationHandlerContext context)
    {
        if (context.User.HasClaim(AuthConstants.AdminUserClaimName, "true"))
        {
            context.Succeed(this);
            return Task.CompletedTask;
        }

        if (context.Resource is not HttpContext httpContext)
        {
            return Task.CompletedTask;
        }
        
        if (!httpContext.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, out
                StringValues extractedApiKey))
        {
            context.Fail();
            return Task.CompletedTask;
        }

        if (_apiKey != extractedApiKey)
        {
            context.Fail();
            return Task.CompletedTask;
        }

        ClaimsIdentity identity = (ClaimsIdentity)httpContext.User.Identity!;
        // Admin GUID can be handled in a better way rather than a magic string here
        identity.AddClaim(new Claim("userid", Guid.Parse("74e20de1-8dd0-4bc2-a9f5-8aa3203ad209").ToString()));
        context.Succeed(this);

        return Task.CompletedTask;
    }
}