using CRM.Application.Features.Authentication.Register;
using CRM.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace CRM.Infrastructure.Authentication;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
    {
        await ValidateEmailAsync(request.Email);

        await ValidateRoleAsync(request.Role);

        var user = CreateApplicationUser(request);

        await CreateIdentityUserAsync(user, request.Password);

        await AssignRoleAsync(user, request.Role);

        return new RegisterResponse
        {
            UserId = user.Id,
            Email = user.Email!,
            Message = "User registered successfully."
        };
    }

    private async Task ValidateEmailAsync(string email)
    {
        var existingUser = await _userManager.FindByEmailAsync(email);

        if (existingUser != null)
        {
            throw new Exception("Email already exists.");
        }
    }

    private async Task ValidateRoleAsync(string role)
    {
        var roleExists = await _roleManager.RoleExistsAsync(role);

        if (!roleExists)
        {
            throw new Exception($"Role '{role}' does not exist.");
        }
    }

    private static ApplicationUser CreateApplicationUser(RegisterRequest request)
    {
        return new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Department = request.Department,
            Designation = request.Designation,
            EmailConfirmed = true,
            IsActive = true
        };
    }

    private async Task CreateIdentityUserAsync(
        ApplicationUser user,
        string password)
    {
        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ",
                result.Errors.Select(e => e.Description));

            throw new Exception(errors);
        }
    }

    private async Task AssignRoleAsync(
        ApplicationUser user,
        string role)
    {
        var result = await _userManager.AddToRoleAsync(user, role);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ",
                result.Errors.Select(e => e.Description));

            throw new Exception(errors);
        }
    }
}