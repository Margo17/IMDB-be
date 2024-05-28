namespace IMDB.Api.Auth;

public static class AuthConstants
{
    public const string AdminUserPolicyName = "Admin";
    public const string AdminUserClaimName = "admin";
    
    public const string TrustedMemberClaimName = "Trusted";
    public const string TrustedMemberPolicyName = "trusted_member";
}