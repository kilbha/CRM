using Microsoft.AspNetCore.Identity;
using CRM.Infrastructure.Configuration;
using Microsoft.Extensions.Options;


namespace CRM.Infrastructure.Identity;

public class IdentitySeeder
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    private readonly SeedDataSettings _seedData;

    public IdentitySeeder(
    RoleManager<ApplicationRole> roleManager,
    UserManager<ApplicationUser> userManager,
    IOptions<SeedDataSettings> seedData)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _seedData = seedData.Value;
    }


    public async Task SeedAsync()
    {
        await SeedRolesAsync();

        await SeedSuperAdminAsync();
    }

    private async Task SeedRolesAsync()
    {
        string[] roles =
        {
            IdentityConstants.SuperAdmin,
            IdentityConstants.Admin,
            IdentityConstants.SalesManager,
            IdentityConstants.SalesExecutive,
            IdentityConstants.SupportExecutive,
            IdentityConstants.Viewer
        };

        foreach (var role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(
                    new ApplicationRole
                    {
                        Name = role,
                        Description = $"{role} Role"
                    });
            }
        }
    }

    private async Task SeedSuperAdminAsync()
    {
        string email = _seedData.AdminEmail;

        var password = _seedData.AdminPassword;

        var user = await _userManager.FindByEmailAsync(email);

        if (user != null)
            return;

        user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = "Super",
            LastName = "Admin",
            EmailConfirmed = true,
            IsActive = true
        };

        var result = await _userManager.CreateAsync(
            user,
            password);

        if (!result.Succeeded)
            throw new Exception(
                string.Join(",",
                result.Errors.Select(x => x.Description)));

        await _userManager.AddToRoleAsync(
            user,
            IdentityConstants.SuperAdmin);
    }
}