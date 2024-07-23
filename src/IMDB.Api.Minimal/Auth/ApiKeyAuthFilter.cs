using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace IMDB.Api.Minimal.Auth;

public class ApiKeyAuthFilter(IConfiguration _configuration) : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName,
                out StringValues extractedApiKey))
        {
            context.Result = new UnauthorizedObjectResult("API key missing");
            return;
        }

        if (extractedApiKey != _configuration["ApiKey"])
        {
            context.Result = new UnauthorizedObjectResult("API key missing");
        }
    }
}