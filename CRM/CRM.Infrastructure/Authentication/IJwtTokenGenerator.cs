using CRM.Infrastructure.Identity;

namespace CRM.Infrastructure.Authentication;

public interface IJwtTokenGenerator
{
    string GenerateToken(ApplicationUser user,
                         IList<string> roles);
}