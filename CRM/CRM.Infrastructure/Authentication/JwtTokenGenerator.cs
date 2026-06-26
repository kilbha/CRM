using CRM.Infrastructure.Identity;

namespace CRM.Infrastructure.Authentication;

public class JwtTokenGenerator
    : IJwtTokenGenerator
{
    public string GenerateToken(
        ApplicationUser user,
        IList<string> roles)
    {
        throw new NotImplementedException();
    }
}