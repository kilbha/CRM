using CRM.Infrastructure.Identity;

namespace CRM.Infrastructure.Authentication;

public interface IJwtTokenGenerator
{
    JwtTokenResult GenerateToken(
        ApplicationUser user,
        IList<string> roles);
}

public class JwtTokenResult
{
    public string Token { get; init; } = string.Empty;

    public DateTime ExpiresAt { get; init; }
}