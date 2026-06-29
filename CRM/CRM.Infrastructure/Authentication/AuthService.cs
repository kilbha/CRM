using CRM.Application.Features.Authentication.Register;
using CRM.Application.Features.Authentication.Login;
using CRM.Application.Features.Authentication.Interfaces;
using CRM.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using CRM.Shared.Exceptions;
using Microsoft.Extensions.Logging;

namespace CRM.Infrastructure.Authentication;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IJwtTokenGenerator jwtTokenGenerator,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtTokenGenerator = jwtTokenGenerator;
        _logger = logger;
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
            _logger.LogWarning("Email '{Email}' is already in use.", email);
            throw new DuplicateResourceException("Email already exists.");
        }
    }

    private async Task ValidateRoleAsync(string role)
    {
        var roleExists = await _roleManager.RoleExistsAsync(role);

        if (!roleExists)
        {
            _logger.LogWarning("Role '{Role}' does not exist.", role);
            throw new NotFoundException($"Role '{role}' does not exist.");
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

            _logger.LogWarning("Failed to create user '{Email}': {Errors}",
                user.Email, errors);

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

            _logger.LogWarning("Failed to assign role '{Role}' to user '{UserId}': {Errors}",
                role, user.Id, errors);

            throw new Exception(errors);
        }
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await GetValidatedUserAsync(request);

        var roles = await GetUserRolesAsync(user);

        var jwt = _jwtTokenGenerator.GenerateToken(user, roles);

        await UpdateLastLoginAsync(user);

        _logger.LogInformation("User '{Email}' logged in successfully.", user.Email);
        return CreateLoginResponse(user, roles, jwt);
    }

    private async Task<ApplicationUser> GetValidatedUserAsync(
    LoginRequest request)
    {
        var user =
            await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            _logger.LogWarning("User with email '{Email}' not found.", request.Email);
            throw new UnauthorizedException(
                "Invalid email or password.");
        }

        if (!user.IsActive)
        {
            _logger.LogWarning("User account with email '{Email}' is inactive.", request.Email);
            throw new ForbiddenException(
                "User account is inactive.");
        }

        var valid =
            await _userManager.CheckPasswordAsync(
                user,
                request.Password);

        if (!valid)
        {
            _logger.LogWarning("Invalid password for user with email '{Email}'.", request.Email);
            throw new UnauthorizedException(
                "Invalid email or password.");
        }

        return user;
    }

    private async Task<IList<string>> GetUserRolesAsync(
    ApplicationUser user)
    {
        return await _userManager.GetRolesAsync(user);
    }

    private async Task UpdateLastLoginAsync(
    ApplicationUser user)
    {
        user.LastLoginAt = DateTime.UtcNow;

        await _userManager.UpdateAsync(user);
    }

    private static LoginResponse CreateLoginResponse(
    ApplicationUser user,
    IList<string> roles,
    JwtTokenResult jwt)
    {
        
        return new LoginResponse
        {
            Token = jwt.Token,

            ExpiresAt = jwt.ExpiresAt,

            Email = user.Email!,

            Roles = roles
        };
    }

}