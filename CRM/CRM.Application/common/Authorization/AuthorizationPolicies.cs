namespace CRM.Application.Common.Authorization;

public static class AuthorizationPolicies
{
    public const string RequireSuperAdmin = nameof(RequireSuperAdmin);

    public const string RequireAdmin = nameof(RequireAdmin);

    public const string RequireSales = nameof(RequireSales);

    public const string RequireSupport = nameof(RequireSupport);
}