using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;

namespace IMDB.Api.Sdk.Consumer;

public class AuthTokenProvider(HttpClient _httpClient)
{
    private string _cachedToken = string.Empty;
    
    // Helps handle thread safety in async context, allows one request at a time when generating token
    private static readonly SemaphoreSlim Lock = new(1, 1);

    public async Task<string> GetTokenAsync()
    {
        if (!string.IsNullOrEmpty(_cachedToken))
        {
            JwtSecurityToken jwt = new JwtSecurityTokenHandler().ReadJwtToken(_cachedToken);
            string expiryTimeText = jwt.Claims.Single(claim => claim.Type == "exp").Value; // expiry claim
            DateTime expiryDateTime = UnixTimeStampToDateTime(int.Parse(expiryTimeText));
            if (expiryDateTime > DateTime.UtcNow)
            {
                return _cachedToken;
            }
        }

        await Lock.WaitAsync();
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync("https://localhost:5003/token", new
        {
            userid = "d8566de3-b1a6-4a9b-b842-8e3887a82e42",
            email = "mg@gmail.com",
            customClaims = new Dictionary<string, object>
            {
                ["admin"] = true,
                ["trusted_member"] = true
            }
        });
        string newToken = await response.Content.ReadAsStringAsync();
        _cachedToken = newToken;
        Lock.Release();

        return newToken;
    }

    private static DateTime UnixTimeStampToDateTime(int unixTimeStamp)
    {
        DateTime dateTime = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        return dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
    }
}